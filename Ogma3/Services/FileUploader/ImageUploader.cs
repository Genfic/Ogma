using B2Net;
using B2Net.Models;
using Ogma3.Data;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Logging.OperationTiming;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Ogma3.Services.FileUploader;

public sealed class ImageUploader(IB2Client b2Client, OgmaConfig ogmaConfig, ILogger<ImageUploader> logger) : IFileUploader
{
	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="Exception">Thrown when after `tries` number of tries, the file could not be uploaded</exception>
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
	/// <exception cref="FileUploadException">Thrown when after `tries` number of tries, the file could not be uploaded</exception>
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

		if ((width ?? height) is {} w && (height ?? width) is {} h)
		{
			using var op = logger.TimeOperation("Resizing image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			// Reset memory stream position
			ms.Seek(0, SeekOrigin.Begin);

			// Load and resize the image
			using var img = await Image.LoadAsync(ms);
			img.Mutate(i => i.Resize(new ResizeOptions
			{
				Size = new Size(w, h),
				Mode = ResizeMode.Crop,
				Position = AnchorPositionMode.Center,
			}));

			// Save it as PNG
			ms.Seek(0, SeekOrigin.Begin);
			await img.SaveAsync(ms, new WebpEncoder());
			ext = "webp";
		}

		// Assemble the final path
		var fileName = $"{folder}/{name}.{ext}";

		// Try to upload the image to the bucket
		var counter = tries;
		while (counter >= 0)
		{
			using var op = logger.TimeOperation("Uploading image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			try
			{
				var result = await b2Client.Files.Upload(ms.ToArray(), fileName);
				return new FileUploaderResult(result.FileId, fileName);
			}
			catch (B2Exception e)
			{
				logger.LogError("⚠ Backblaze Error: {Message}\n\tTries left: {Count}", e.Message, --counter);
			}
		}

		logger.LogError("Couldn't upload file {Name} ({Size} bytes)", file.Name, file.Length);
		throw new FileUploadException("Could not upload file. Check server logs.", file.Name, file.Length);
	}

	public async Task Delete(string name, string id, CancellationToken cancellationToken = default)
		=> _ = await b2Client.Files.Delete(id, name.Replace(ogmaConfig.Cdn, string.Empty).Trim('/'), cancellationToken);
}