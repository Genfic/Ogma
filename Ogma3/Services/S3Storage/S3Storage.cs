using Amazon.Runtime;
using Amazon.S3;

namespace Ogma3.Services.S3Storage;

public static class S3Storage
{
	public static IServiceCollection AddS3Storage(this IServiceCollection services, IConfiguration config)
	{
		var options = config.GetSection("B2").Get<S3StorageOptions>() ?? throw new InvalidOperationException("S3 storage options not found");

		services.AddSingleton<IAmazonS3>(_ => {
			var s3Config = new AmazonS3Config
			{
				ServiceURL = options.ServiceUrl,
				ForcePathStyle = false,
			};

			var credentials = new BasicAWSCredentials(options.KeyId, options.ApplicationKey);

			return new AmazonS3Client(credentials, s3Config);
		});
		return services;
	}
}