using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services.Hosted;

public sealed class InfractionRemover(ILogger<InfractionRemover> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			logger.LogInformation("Worker {name} running at: {time}", nameof(InfractionRemover), DateTimeOffset.Now);
			await DoWork(stoppingToken);
			await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
		}
	}

	private async Task DoWork(CancellationToken cancellationToken)
	{
		using var scope = scopeFactory.CreateScope();
		await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		var result = await context.Infractions
			.Where(i => i.ActiveUntil < DateTimeOffset.UtcNow)
			.ExecuteUpdateAsync(
				s => s.SetProperty(i => i.RemovedAt, DateTimeOffset.UtcNow),
				cancellationToken
			);

		logger.LogInformation("Automatically rescinded {result} expired infractions", result);
	}
}