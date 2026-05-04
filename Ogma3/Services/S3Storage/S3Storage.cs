using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;

namespace Ogma3.Services.S3Storage;

public static class S3Storage
{
	public static IServiceCollection AddS3Storage(this IServiceCollection services, IConfiguration config)
	{
		services
			.AddOptions<S3StorageOptions>()
			.Bind(config.GetSection("B2"))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services.AddSingleton<IAmazonS3>(s => {
			var options = s.GetRequiredService<IOptions<S3StorageOptions>>().Value;

			var s3Config = new AmazonS3Config
			{
				ServiceURL = options.ServiceUrl,
				ForcePathStyle = true,
			};

			var credentials = new BasicAWSCredentials(options.KeyId, options.ApplicationKey);

			return new AmazonS3Client(credentials, s3Config);
		});
		return services;
	}
}