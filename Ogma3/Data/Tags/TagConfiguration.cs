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
			.HasIndex(t => t.Name);

		builder
			.Property(t => t.Name)
			.IsCitext()
			.IsRequired()
			.HasMaxLength(CTConfig.Tag.MaxNameLength);

		builder
			.Property(t => t.Slug)
			.IsRequired()
			.HasMaxLength(CTConfig.Tag.MaxNameLength);

		builder
			.Property(t => t.Description)
			.HasMaxLength(CTConfig.Tag.MaxDescLength);

		builder
			.Property(t => t.Namespace)
			.IsRequired(false)
			.HasDefaultValue(null);
	}
}