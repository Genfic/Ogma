using Amazon.S3;
using Amazon.S3.Model;
using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Logging.OperationTiming;
using Ogma3.Services.S3Storage;

namespace Ogma3.Services.FileUploader;

[RegisterSingleton<IFileUploader>]
[UsedImplicitly]
public sealed class ImageUploader(IAmazonS3 s3Client, IOptions<S3StorageOptions> options, ILogger<ImageUploader> logger) : IFileUploader
{
	private readonly S3StorageOptions _s3Options = options.Value;

	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="Exception">Thrown when after `tries` number of tries, the file could not be uploaded</exception>
	public async Task<FileUploadResult> Upload(
		byte[] bytes,
		string folder,
		int tries = 5
	)
	{
		var name = Guid.NewGuid().ToString();
		return await Upload(bytes, folder, name, tries);
	}

	/// <inheritdoc cref="IFileUploader"/>
	/// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
	/// <exception cref="FileUploadException">Thrown when after `tries` number of tries, the file could not be uploaded</exception>
	public async Task<FileUploadResult> Upload(
		byte[] bytes,
		string folder,
		string name,
		int tries = 5
	)
	{
		// Assemble the final path
		var key = $"{folder}/{name}.webp";

		// Try to upload the image to the bucket
		var counter = tries;
		while (counter >= 0)
		{
			using var op = logger.TimeOperation("Uploading image {Filename} that weighs {Size} bytes", name, bytes.Length);

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

		logger.LogError("Couldn't upload file {Name} ({Size} bytes)", name, bytes.Length);
		throw new FileUploadException("Could not upload file. Check server logs.", name, bytes.Length);
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
}