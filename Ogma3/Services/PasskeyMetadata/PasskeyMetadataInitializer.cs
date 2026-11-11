namespace Ogma3.Services.PasskeyMetadata;

public sealed class PasskeyMetadataInitializer(PasskeyMetadataService service) : IHostedService
{
	public Task StartAsync(CancellationToken cancellationToken) => service.WarmUpAsync(cancellationToken);

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
