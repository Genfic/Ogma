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
		{
			throw new ArgumentException("File cannot be null or empty");
		}

		// Load the image to a memory stream
		await using var inputMs = new MemoryStream();
		await file.CopyToAsync(inputMs);
		inputMs.Seek(0, SeekOrigin.Begin);

		using var img = await Image.LoadAsync(inputMs);

		// Remove all but the second frame if the image is animated
		if (img.Frames.Count > 1)
		{
			img.Frames.RemoveFrame(0);

			while (img.Frames.Count > 1)
			{
				img.Frames.RemoveFrame(1);
			}
		}

		if ((width ?? height) is {} w && (height ?? width) is {} h)
		{
			using var op = logger.TimeOperation("Resizing image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			// Load and resize the image
			img.Mutate(i => i.Resize(new ResizeOptions
			{
				Size = new Size(w, h),
				Mode = ResizeMode.Crop,
				Position = AnchorPositionMode.Center,
			}));
		}

		// Strip EXIF metadata
		img.Metadata.ExifProfile = null;

		// Save it as WEBP
		await using var outputMs = new MemoryStream();
		await img.SaveAsync(outputMs, new WebpEncoder());

		// Assemble the final path
		var fileName = $"{folder}/{name}.webp";

		// Try to upload the image to the bucket
		var counter = tries;
		while (counter >= 0)
		{
			using var op = logger.TimeOperation("Uploading image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			try
			{
				var uploadUrl = await b2Client.Files.GetUploadUrl();
				var uploadContext = new B2FileUploadContext
				{
					FileName = fileName,
					B2UploadUrl = uploadUrl,
				};

				var result = await b2Client.Files.Upload(outputMs.ToArray(), uploadContext);
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