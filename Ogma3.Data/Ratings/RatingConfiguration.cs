using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;

namespace Ogma3.Data.Ratings;

public sealed class RatingConfiguration : BaseConfiguration<Rating>
{
	protected override void Config(EntityTypeBuilder<Rating> builder)
	{

		// CONSTRAINTS
		builder
			.HasIndex(r => r.Name)
			.IsUnique();

		builder
			.Property(r => r.Name)
			.IsRequired()
			.UseCollation(PgConstants.CollationNames.CaseInsensitiveNoAccent)
			.HasMaxLength(CTConfig.Rating.MaxNameLength);

		builder
			.Property(r => r.Description)
			.IsRequired()
			.HasMaxLength(CTConfig.Rating.MaxDescriptionLength);

		builder
			.Property(r => r.Color)
			.HasMaxLength(6);

		builder
			.Property(r => r.Order)
			.IsRequired()
			.HasDefaultValue(0);

		builder
			.Property(r => r.BlacklistedByDefault)
			.IsRequired()
			.HasDefaultValue(false);
	}
}