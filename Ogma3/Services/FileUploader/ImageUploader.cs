using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Logging.OperationTiming;
using Ogma3.Services.S3Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Ogma3.Services.FileUploader;

[RegisterSingleton]
public sealed class ImageUploader(IAmazonS3 s3Client, IOptions<S3StorageOptions> options, ILogger<ImageUploader> logger) : IFileUploader
{
	private readonly S3StorageOptions _s3Options = options.Value;

	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="Exception">Thrown when after `tries` number of tries, the file could not be uploaded</exception>
	public async Task<FileUploadResult> Upload(
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
	public async Task<FileUploadResult> Upload(
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

		await using var outputMs = await ProcessImage(file, width, height);
		outputMs.Position = 0;
		var bytes = outputMs.ToArray();

		// Assemble the final path
		var key = $"{folder}/{name}.webp";

		// Try to upload the image to the bucket
		var counter = tries;
		while (counter >= 0)
		{
			using var op = logger.TimeOperation("Uploading image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

			try
			{
				using var uploadStream = new MemoryStream(bytes);

				var request = new PutObjectRequest
				{
					BucketName = _s3Options.BucketName,
					Key = key,
					InputStream = uploadStream,
					ContentType = "image/webp",
				};
				var res = await s3Client.PutObjectAsync(request);

				return new(key, res.ETag.Trim('"'));
			}
			catch (AmazonS3Exception e)
			{
				logger.LogError("⚠ Backblaze Error: {Message}\n\tTries left: {Count}", e.Message, counter--);
			}
		}

		logger.LogError("Couldn't upload file {Name} ({Size} bytes)", file.Name, file.Length);
		throw new FileUploadException("Could not upload file. Check server logs.", file.Name, file.Length);
	}

	public async Task Delete(string key, CancellationToken cancellationToken = default)
	{
		var request = new DeleteObjectRequest
		{
			BucketName = _s3Options.BucketName,
			Key = key,
		};
		await s3Client.DeleteObjectAsync(request, cancellationToken);
	}

	private async Task<MemoryStream> ProcessImage(IFormFile file, int? width = null, int? height = null)
	{
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

		if ((width ?? height) is { } w && (height ?? width) is { } h && (img.Width > w || img.Height > h))
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
		var outputMs = new MemoryStream();
		await img.SaveAsync(outputMs, new WebpEncoder());

		return outputMs;
	}
}