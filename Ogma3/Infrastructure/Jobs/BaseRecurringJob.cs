namespace Ogma3.Infrastructure.Jobs;

public abstract class BaseRecurringJob(IServiceProvider serviceProvider, ILogger logger) : BackgroundService
{
	protected readonly IServiceProvider ServiceProvider = serviceProvider;
	protected readonly ILogger Logger = logger;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{

		try
		{
			await DelayUntilNextBoundary(stoppingToken);

			using var timer = new PeriodicTimer(Interval);

			while (await timer.WaitForNextTickAsync(stoppingToken))
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
		catch (OperationCanceledException)
		{
			Logger.LogInformation("Job [{Name}] stopped", Name);
		}
	}

	private Task DelayUntilNextBoundary(CancellationToken ct)
	{
		var intervalTicks = Interval.Ticks;
		if (intervalTicks <= 0)
		{
			return Task.CompletedTask;
		}

		var nowTicks = DateTime.UtcNow.Ticks;
		var remainder = nowTicks % intervalTicks;
		if (remainder <= 0)
		{
			return Task.CompletedTask;
		}

		var delay = TimeSpan.FromTicks(intervalTicks - remainder);
		return Task.Delay(delay, ct);
	}

	protected abstract TimeSpan Interval { get; }

	protected abstract string Name { get; }

	protected abstract Task Run(CancellationToken ct);

	protected virtual bool AlignToClock => true;
}