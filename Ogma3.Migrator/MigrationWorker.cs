using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NetEscapades.EnumGenerators;
using Ogma3.Data;

namespace Ogma3.Migrator;

public partial class MigrationWorker
(
	ILogger<MigrationWorker> logger,
	IServiceProvider services,
	IHostApplicationLifetime appLifetime
) : BackgroundService
{
	public const string ActivitySourceName = nameof(MigrationWorker);
	private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var watch = Stopwatch.StartNew();
		using var activity = ActivitySource.StartActivity(ActivityKind.Client);

		try
		{
			logger.LogInformation("Migrating database...");

			await using var scope = services.CreateAsyncScope();
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			var squashResult = await SquashAsync(db, stoppingToken);

			if (squashResult == SquashResult.Failed)
			{
				logger.LogError("Squash migration failed.");
				Environment.ExitCode = 1;
				return;
			}

			LogSquashMigrationWasStatus(squashResult.ToStringFast());

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
			LogMigrationTookElapsedMs(watch.ElapsedMilliseconds);
		}
	}

	private async Task<SquashResult> SquashAsync(AppDbContext ctx, CancellationToken ct)
	{
		var migrations = ctx.Database.GetMigrations().ToArray();

		if (migrations.Length != 1 || !migrations[0].Contains("squash", StringComparison.OrdinalIgnoreCase))
		{
			return SquashResult.NotNeeded;
		}

		var squashMigration = migrations[0];
		var applied = (await ctx.Database.GetAppliedMigrationsAsync(ct)).ToArray();

		if (applied.Length == 0 || applied.Contains(squashMigration))
		{
			return SquashResult.AlreadyDone;
		}

		var efVersion = typeof(DbContext).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
			?.InformationalVersion.Split('+')[0];

		if (string.IsNullOrWhiteSpace(efVersion))
		{
			logger.LogWarning("EF version could not be determined, skipping squash migration.");
			return SquashResult.Failed;
		}

		await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

		await ctx.Database.ExecuteSqlRawAsync(
			"""
			DELETE FROM "__EFMigrationsHistory" WHERE TRUE;
			""",
			ct);

		var count = await ctx.Database.ExecuteSqlAsync(
			$"""
			 INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
			 VALUES ({squashMigration}, {efVersion});
			 """,
			ct);

		if (count != 1)
		{
			LogSquashMigrationAffectedCountRowsRollingBack(count);
			return SquashResult.Failed;
		}

		await transaction.CommitAsync(ct);

		return SquashResult.Completed;
	}

	[EnumExtensions]
	internal enum SquashResult
	{
		NotNeeded,
		AlreadyDone,
		Completed,
		Failed,
	}

	[LoggerMessage(LogLevel.Information, "Migration took {Elapsed} ms")]
	partial void LogMigrationTookElapsedMs(long elapsed);

	[LoggerMessage(LogLevel.Information, "Squash migration was {Status}")]
	partial void LogSquashMigrationWasStatus(string status);

	[LoggerMessage(LogLevel.Warning, "Squash migration affected {Count} rows. Rolling back.")]
	partial void LogSquashMigrationAffectedCountRowsRollingBack(int count);
}