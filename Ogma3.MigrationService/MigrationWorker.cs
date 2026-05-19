using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.MigrationService;

public sealed class MigrationWorker(
	IServiceProvider serviceProvider,
	IHostApplicationLifetime hostApplicationLifetime,
	ILogger<MigrationWorker> logger
) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			await using var scope = serviceProvider.CreateAsyncScope();
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			var strategy = context.Database.CreateExecutionStrategy();
			await strategy.ExecuteAsync((logger, context), async state => {
				state.logger.LogInformation("Applying database migrations.");
				await state.context.Database.MigrateAsync(stoppingToken);
				state.logger.LogInformation("Database migrations applied.");
			});
		}
		catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
		{
			logger.LogInformation("Database migration was cancelled.");
		}
		catch (Exception ex)
		{
			logger.LogCritical(ex, "Database migration failed.");
			Environment.ExitCode = 1;
		}
		finally
		{
			hostApplicationLifetime.StopApplication();
		}
	}
}
