using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Ogma3.AppHost.Helpers;

public static class DbDumper
{
	private static async Task<ExecuteCommandResult> Dump(ExecuteCommandContext ctx, IResourceBuilder<PostgresServerResource> builder)
	{
		var notifications = ctx.Services.GetRequiredService<ResourceNotificationService>();

		if (!notifications.TryGetCurrentState(builder.Resource.Name, out var current)
		    || current.Snapshot.State?.Text != KnownResourceStates.Running
		    || current.Snapshot.Properties.FirstOrDefault(p => p.Name == "container.id")?.Value is not string { Length: > 0 } containerId)
		{
			return CommandResults.Failure("The PostgreSQL container isn't running or has no Docker ID");
		}

		var directory = Path.GetFullPath(Path.Combine(builder.ApplicationBuilder.AppHostDirectory, "..", ".backups", "dumps"));
		var path = Path.Combine(directory, $"ogma3-db-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss-fff}Z.dump");
		var part = $"{path}.{Guid.CreateVersion7()}.part";

		try
		{
			Directory.CreateDirectory(directory);

			var start = new ProcessStartInfo("docker")
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
				ArgumentList =
				{
					"exec", containerId, "sh", "-c",
					"""
					export PGPASSWORD=$POSTGRES_PASSWORD;
					exec pg_dump --no-password --format=custom --username="${POSTGRES_USER:-postgres}" --dbname=ogma3-db
					""",
				}
			};

			await using (var output = File.Create(part))
			{
				using var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start Docker.");
				await using var cancellation = ctx.CancellationToken.Register(static state => {
					try
					{
						(state as Process)?.Kill(entireProcessTree: true);
					}
					catch (InvalidOperationException)
					{
						// Already exited
					}
				}, process);

				var stderr = process.StandardError.ReadToEndAsync(ctx.CancellationToken);
				await Task.WhenAll(
					process.StandardOutput.BaseStream.CopyToAsync(output, ctx.CancellationToken),
					stderr,
					process.WaitForExitAsync(ctx.CancellationToken)
				);

				if (process.ExitCode != 0)
				{
					return CommandResults.Failure($"pg_dump exited with code {process.ExitCode}: {(await stderr).Trim()}");
				}
			}

			ctx.CancellationToken.ThrowIfCancellationRequested();
			File.Move(part, path);

			return CommandResults.Success($"Saved backup to {path}");
		}
		catch (OperationCanceledException) when (ctx.CancellationToken.IsCancellationRequested)
		{
			return CommandResults.Canceled();
		}
		catch (Exception ex)
		{
			return CommandResults.Failure($"pg_dump failed: {ex.Message}");
		}
		finally
		{
			try
			{
				File.Delete(part);
			}
			catch (IOException) {}
			catch (UnauthorizedAccessException) {}
		}
	}

	public static IResourceBuilder<PostgresServerResource> WithDbDump(this IResourceBuilder<PostgresServerResource> builder)
	{
		if (!builder.ApplicationBuilder.ExecutionContext.IsRunMode)
		{
			return builder;
		}

		return builder
			.WithCommand(
				"dump-db",
				"Dump database",
				ctx => Dump(ctx, builder),
				new CommandOptions
				{
					IsHighlighted = true,
					IconName = "DatabaseArrowDown",
					IconVariant = IconVariant.Regular,
				}
			);
	}
}