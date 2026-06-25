using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Migrator;

public class MigrationWorker(
	ILogger<MigrationWorker> logger,
	IServiceProvider services,
	IHostApplicationLifetime appLifetime
	) : BackgroundService
{
	public const string ActivitySourceName = nameof(MigrationWorker);
	private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var activity = ActivitySource.StartActivity(ActivityKind.Client);

		try
		{
			logger.LogInformation("Migrating database...");

			using var scope = services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			await db.Database.MigrateAsync(stoppingToken);

			logger.LogInformation("Database migration complete.");
		}
		catch (Exception ex)
		{
			activity?.AddException(ex);
			logger.LogError(ex, "An error occurred while migrating the database.");
			Environment.ExitCode = 1;
		}
		finally
		{
			appLifetime.StopApplication();
		}
	}
}