using Amazon.S3;
using Amazon.S3.Model;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using NetVips;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Logging.OperationTiming;
using Ogma3.Services.S3Storage;

namespace Ogma3.Services.FileUploader;

[RegisterSingleton]
[UsedImplicitly]
public sealed class ImageUploader(IAmazonS3 s3Client, IOptions<S3StorageOptions> options, ILogger<ImageUploader> logger) : IFileUploader
{
	private readonly S3StorageOptions _s3Options = options.Value;

	private const int MaxFrames = 100;

	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="Exception">Thrown when after `tries` number of tries, the file could not be uploaded</exception>
	public async Task<FileUploadResult> Upload(
		IFormFile file,
		string folder,
		int? width = null,
		int? height = null,
		int tries = 5,
		bool allowAnimated = false
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
		int tries = 5,
		bool allowAnimated = false
	)
	{
		if (file is not { Length: > 0 })
		{
			throw new ArgumentException("File cannot be null or empty");
		}

		await using var outputMs = await ProcessImageVips(file, width, height, allowAnimated);
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

	private async Task<MemoryStream> ProcessImageVips(IFormFile file, int? width, int? height, bool allowAnimated)
	{
		using var op = logger.TimeOperation("Resizing image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

		await using var stream = file.OpenReadStream();
		var bytes = new byte[stream.Length];
		await stream.ReadExactlyAsync(bytes, 0, (int)stream.Length);

		var w = width ?? height ?? throw new ArgumentException($"At least one of {nameof(width)} and {nameof(height)} should not be null");
		var h = height ?? w;

		return await Task.Run(() => {
			using var processed = Image.ThumbnailBuffer(bytes,
				width: w,
				height: h,
				size: Enums.Size.Down,
				crop: Enums.Interesting.Centre,
				optionString: allowAnimated ? $"[n={MaxFrames}]" : "[n=1]"
			);

			var saveOptions = processed.Contains("n-pages") && (processed.Get("n-pages") as int?) is > 1
				? ".webp[Q=82,strip=true,loop=0]"
				: ".webp[Q=85,strip=true]";

			var outStream = new MemoryStream();
			processed.WriteToStream(outStream, saveOptions);
			outStream.Position = 0;
			return outStream;
		});
	}
}