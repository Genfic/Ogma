using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Http;
using Ogma3.Data;
using Serilog;
using SerilogTimings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Ogma3.Services.FileUploader;

public class ImageUploader(IB2Client b2Client, OgmaConfig ogmaConfig) : IFileUploader
{
	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="Exception">Thrown when after `tries` amount of tries file could not be uploaded</exception>
	public async Task<FileUploaderResult> Upload(
		IFormFile file,
		string folder,
		int? width = null,
		int? height = null,
		int tries = 5
	)
	{
		var name = Guid.NewGuid().ToString();
		return await Upload(file, folder, name, width, height, tries);
	}

	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="Exception">Thrown when after `tries` amount of tries file could not be uploaded</exception>
	/// TODO: Remove in favour of the method that generates a GUID for name
	public async Task<FileUploaderResult> Upload(
		IFormFile file,
		string folder,
		string name,
		int? width = null,
		int? height = null,
		int tries = 5
	)
	{
		if (file is not { Length: > 0 })
			throw new ArgumentException("File cannot be null or empty");

		// Read file extension
		var ext = file.FileName.Split('.').Last();

		// Load the image to a memory stream
		await using var ms = new MemoryStream();
		await file.CopyToAsync(ms);

		if (width is not null || height is not null)
		{
			using var op = Operation.Time("Resizing image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			// Reset memory stream position
			ms.Seek(0, SeekOrigin.Begin);

			// Load and resize the image
			using var img = await Image.LoadAsync(ms);
			img.Mutate(i => i.Resize(new ResizeOptions
			{
				Size = new Size((int)(width ?? height)!, (int)(height ?? width)!),
				Mode = ResizeMode.Crop,
				Position = AnchorPositionMode.Center
			}));

			// Save it as PNG
			ms.Seek(0, SeekOrigin.Begin);
			await img.SaveAsync(ms, new PngEncoder());
			ext = "png";
		}

		// Assemble the final path
		var fileName = $"{folder}/{name}.{ext}";

		// Try to upload the image to the bucket
		var counter = tries;
		while (counter >= 0)
		{
			using var op = Operation.Time("Uploading image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			try
			{
				var result = await b2Client.Files.Upload(ms.ToArray(), fileName);
				return new FileUploaderResult(result.FileId, fileName);
			}
			catch (B2Exception e)
			{
				Log.Error("⚠ Backblaze Error: {Message}\n\tTries left: {Count}", e.Message, --counter);
			}
		}

		Log.Error("Couldn't upload file {Name} ({Size} bytes)", file.Name, file.Length);
		throw new Exception("Could not upload file. Check server logs.");
	}

	public async Task Delete(string name, string id, CancellationToken cancellationToken = default)
		=> _ = await b2Client.Files.Delete(id, name.Replace(ogmaConfig.Cdn, string.Empty).Trim('/'), cancellationToken);
}