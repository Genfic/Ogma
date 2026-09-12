using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Ogma3.Data;

public interface ISchedulableContent
{
	DateTimeOffset? ScheduledFor { get; init; }
}

public sealed class ISchedulableContentConvention : IModelFinalizingConvention
{
	public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
	{
		foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
		{
			if (typeof(ISchedulableContent).IsAssignableFrom(entityType.ClrType))
			{
				entityType.Builder
					.HasIndex([nameof(ISchedulableContent.ScheduledFor)])?
					.HasFilter($"\"{nameof(ISchedulableContent.ScheduledFor)}\" IS NOT NULL");
			}
		}
	}
}