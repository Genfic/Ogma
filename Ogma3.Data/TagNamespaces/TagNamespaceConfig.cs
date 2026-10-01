using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;
using Ogma3.Data.Helpers;

namespace Ogma3.Data.TagNamespaces;

public class TagNamespaceConfig : BaseConfiguration<TagNamespace>
{
	protected override void Config(EntityTypeBuilder<TagNamespace> builder)
	{

		builder
			.HasIndex(t => t.Name)
			.UseCollation(PgConstants.CollationNames.CaseInsensitiveNoAccent)
			.IsUnique();

		builder
			.HasIndex(t => t.Slug)
			.UseCollation(PgConstants.CollationNames.CaseInsensitiveNoAccent)
			.IsUnique();

		builder
			.HasPartialIndex(t => t.Alias)
			.UseCollation(PgConstants.CollationNames.CaseInsensitiveNoAccent)
			.IsUnique();

		builder
			.Property(t => t.Name)
			.IsRequired()
			.HasMaxLength(CTConfig.TagNamespace.MaxNameLength);

		builder
			.Property(t => t.Slug)
			.IsRequired()
			.HasMaxLength(CTConfig.TagNamespace.MaxSlugLength);

		builder
			.Property(t => t.Description)
			.IsRequired(false)
			.HasDefaultValue(null)
			.HasMaxLength(CTConfig.TagNamespace.MaxDescLength);

		builder
			.Property(t => t.Alias)
			.IsRequired(false)
			.HasDefaultValue(null)
			.HasMaxLength(CTConfig.TagNamespace.MaxAliasLength);

		builder
			.Property(t => t.Color)
			.IsRequired(false)
			.HasDefaultValue(null)
			.HasMaxLength(CTConfig.TagNamespace.ColorLength);

		builder.HasData(
			new()
			{
				Id = 1,
				Name = "Content Warning",
				Slug = "content-warning",
				Alias = "cw",
				Color = "d91919",
			},
			new()
			{
				Id = 2,
				Name = "Genre",
				Slug = "genre",
				Alias = "ge",
				Color = "8c37f4",
			},
			new()
			{
				Id = 3,
				Name = "Franchise",
				Slug = "franchise",
				Alias = "fr",
				Color = "18f900",
			}
		);
	}
}
