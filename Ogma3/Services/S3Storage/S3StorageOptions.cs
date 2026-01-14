namespace Ogma3.Services.S3Storage;

public sealed class S3StorageOptions
{
	public required string ServiceUrl { get; init; }
	public required string KeyId { get; init; }
	public required string ApplicationKey { get; init; }
	public required string BucketName { get; init; }
}