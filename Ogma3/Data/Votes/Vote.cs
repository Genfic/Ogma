#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Votes;

public sealed class Vote : BaseModel
{
	public OgmaUser User { get; init; }
	public long UserId { get; init; }
	public long StoryId { get; init; }

	public sealed class VoteConfiguration : BaseConfiguration<Vote>
	{
		public override void Configure(EntityTypeBuilder<Vote> builder)
		{
			base.Configure(builder);

			builder
				.HasOne(v => v.User)
				.WithMany()
				.HasForeignKey(v => v.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasIndex(v => new { v.UserId, v.StoryId })
				.IsUnique();
		}
	}
}