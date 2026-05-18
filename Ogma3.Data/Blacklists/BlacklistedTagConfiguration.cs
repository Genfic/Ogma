using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Data.Blacklists;

public sealed class BlacklistedTagConfiguration : IEntityTypeConfiguration<BlacklistedTag>
{
	public void Configure(EntityTypeBuilder<BlacklistedTag> builder)
	{
		builder
			.HasKey(br => new { br.UserId, br.TagId });

		builder
			.HasOne(e => e.Tag)
			.WithMany();

		builder
			.HasOne(e => e.User)
			.WithMany(u => u.BlacklistedTags);
	}
}