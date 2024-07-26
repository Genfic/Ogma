using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Quotes;

public class Quote : BaseModel
{
	public required string Body { get; init; }
	public required string Author { get; init; }

	public class QuoteConfiguration : BaseConfiguration<Quote>
	{
		public override void Configure(EntityTypeBuilder<Quote> builder)
		{
			base.Configure(builder);
			builder.Property(q => q.Body).IsRequired();
			builder.Property(q => q.Author).IsRequired();
		}
	}
}


[Mapper]
public static partial class QuoteMapper
{
	public static partial IQueryable<QuoteDto> ProjectToDto(this IQueryable<Quote> q);
}