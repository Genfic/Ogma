using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Clubs;

public class ClubBan
{
	public OgmaUser User { get; set; } = null!;
	public long UserId { get; set; }
	public Club Club { get; set; } = null!;
	public long ClubId { get; set; }

	public OgmaUser Issuer { get; set; } = null!;
	public long IssuerId { get; set; }
	public DateTime BanDate { get; set; }
	public string Reason { get; set; } = "";

	public class ClubBanConfiguration : IEntityTypeConfiguration<ClubBan>
	{
		public void Configure(EntityTypeBuilder<ClubBan> builder)
		{
			builder
				.HasKey(cb => new { cb.ClubId, cb.UserId });
			builder
				.Property(cb => cb.BanDate)
				.HasDefaultValueSql(PgConstants.CurrentTimestamp);
			builder
				.Property(cb => cb.Reason)
				.IsRequired();
		}
	}
}