using ConfigBoundNET;

namespace Ogma3.Services.S3Storage;

[ConfigSection("B2")]
public sealed partial class S3StorageOptions
{
	public required string ServiceUrl { get; init; }
	public required string KeyId { get; init; }
	public required string ApplicationKey { get; init; }
	public required string BucketName { get; init; }
	public required string BucketId { get; init; }
}