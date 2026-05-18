using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Quotes;

[AutoDbSet]
public sealed class Quote : BaseModel
{
	public required string Body { get; init; }
	public required string Author { get; init; }

	public sealed class QuoteConfiguration : BaseConfiguration<Quote>
	{
		public override void Configure(EntityTypeBuilder<Quote> builder)
		{
			base.Configure(builder);
			builder.Property(q => q.Body).IsRequired();
			builder.Property(q => q.Author).IsRequired();
		}
	}
}

