using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Data.Blacklists;

public class BlacklistedRatingConfiguration : IEntityTypeConfiguration<BlacklistedRating>
{
	public void Configure(EntityTypeBuilder<BlacklistedRating> builder)
	{
		builder
			.HasKey(br => new { br.UserId, br.RatingId });

		builder
			.HasOne(e => e.Rating)
			.WithMany();

		builder
			.HasOne(e => e.User)
			.WithMany(u => u.BlacklistedRatings);
	}
}