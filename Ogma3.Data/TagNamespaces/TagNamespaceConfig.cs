using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;

namespace Ogma3.Data.TagNamespaces;

public class TagNamespaceConfig : BaseConfiguration<TagNamespace>
{
	public override void Configure(EntityTypeBuilder<TagNamespace> builder)
	{
		base.Configure(builder);

		builder
			.HasIndex(t => t.Name)
			.UseCollation(PgConstants.CollationNames.CaseInsensitiveNoAccent)
			.IsUnique();

		builder
			.HasIndex(t => t.Slug)
			.UseCollation(PgConstants.CollationNames.CaseInsensitiveNoAccent)
			.IsUnique();

		builder
			.Property(t => t.Name)
			.HasMaxLength(32);

		builder
			.Property(t => t.Slug)
			.HasMaxLength(32);

		builder
			.Property(t => t.Description)
			.HasMaxLength(256);

		builder
			.Property(t => t.Alias)
			.HasMaxLength(5);

		builder
			.Property(t => t.Color)
			.HasMaxLength(8);

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