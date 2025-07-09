using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Documents;

public sealed class DocumentConfiguration : BaseConfiguration<Document>
{
	public override void Configure(EntityTypeBuilder<Document> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(d => d.Title)
			.HasMaxLength(500)
			.IsRequired();

		builder
			.Property(d => d.Slug)
			.HasMaxLength(500)
			.IsRequired();

		builder
			.Property(d => d.RevisionDate)
			.HasDefaultValue(null);

		builder
			.Property(d => d.CreationTime)
			.IsRequired()
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder
			.Property(d => d.Version)
			.IsRequired()
			.HasDefaultValue(1);

		builder
			.Property(d => d.Body)
			.IsRequired();

		builder
			.Property(d => d.CompiledBody)
			.IsRequired();

		builder
			.OwnsMany<Document.Header>(
				c => c.Headers,
				d => {
					d.ToJson();
				}
			);

		builder
			.HasIndex(d => new { d.Slug, d.Version })
			.IsUnique();

		builder
			.HasIndex(d => new { d.Title, d.Version })
			.IsUnique();
	}
}