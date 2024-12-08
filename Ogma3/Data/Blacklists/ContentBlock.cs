#nullable disable

using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Blacklists;

[AutoDbSet]
public sealed class ContentBlock : BaseModel
{
	public OgmaUser Issuer { get; init; }
	public long IssuerId { get; init; }
	public string Reason { get; init; }
	public DateTimeOffset DateTime { get; init; }
	public string Type { get; init; }

	public sealed class ContentBlockConfiguration : BaseConfiguration<ContentBlock>
	{
		public override void Configure(EntityTypeBuilder<ContentBlock> builder)
		{
			base.Configure(builder);

			builder
				.Property(cb => cb.Reason)
				.IsRequired();
			builder
				.Property(cb => cb.DateTime)
				.IsRequired()
				.HasDefaultValueSql(PgConstants.CurrentTimestamp);
			builder
				.Property(cb => cb.Type)
				.IsRequired();
		}
	}
}