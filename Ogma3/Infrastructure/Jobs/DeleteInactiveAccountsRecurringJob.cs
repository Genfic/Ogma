using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Jobs;

public sealed class DeleteInactiveAccountsRecurringJob(IServiceProvider serviceProvider, ILogger<DeleteInactiveAccountsRecurringJob> logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromDays(1);
	protected override string Name => nameof(DeleteInactiveAccountsRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		var cutoff = DateTimeOffset.UtcNow - TimeSpan.FromDays(30);

		var count = await ctx.Users
			.Where(u => !u.EmailConfirmed)
			.Where(u => u.RegistrationDate < cutoff)
			.ExecuteDeleteAsync(cancellationToken: ct);

		Logger.LogInformation("Deleted {Count} accounts not activated since {CutoffDate}", count, cutoff);
	}
}