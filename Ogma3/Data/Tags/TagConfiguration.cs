using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Data.Tags;

public sealed class TagConfiguration : BaseConfiguration<Tag>
{
	public override void Configure(EntityTypeBuilder<Tag> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.HasIndex(t => new { t.Name, t.Namespace })
			.IsUnique();

		builder
			.HasIndex(t => t.Name)
			.IsUnique();

		builder
			.HasIndex(t => t.Slug)
			.IsUnique();

		builder
			.Property(t => t.Name)
			.IsCitext()
			.IsRequired()
			.HasMaxLength(CTConfig.Tag.MaxNameLength);

		builder
			.Property(t => t.Slug)
			.IsCitext()
			.IsRequired()
			.HasMaxLength(CTConfig.Tag.MaxNameLength);

		builder
			.Property(t => t.Description)
			.HasMaxLength(CTConfig.Tag.MaxDescLength)
			.IsRequired(false)
			.HasDefaultValue(null);

		builder
			.Property(t => t.Namespace)
			.IsRequired(false)
			.HasDefaultValue(null);
	}
}