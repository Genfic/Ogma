namespace Ogma3.Infrastructure.Jobs;

public abstract class BaseRecurringJob(IServiceProvider serviceProvider, ILogger logger) : BackgroundService
{
	protected readonly IServiceProvider ServiceProvider = serviceProvider;
	protected readonly ILogger Logger = logger;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(Interval);

		while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
		{
			try
			{
				await Run(stoppingToken);
			}
			catch (Exception e)
			{
				Logger.LogError(e, "Error running job [{Name}]", Name);
			}
		}
	}

	protected abstract TimeSpan Interval { get; }

	protected abstract string Name { get; }

	protected abstract Task Run(CancellationToken ct);
}