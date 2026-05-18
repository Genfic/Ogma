using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Quotes;

[Mapper]
public static partial class QuoteMapper
{
	public static partial IQueryable<QuoteDto> ProjectToDto(this IQueryable<Quote> q);
	public static partial IQueryable<FullQuoteDto> ProjectToFullDto(this IQueryable<Quote> q);
}
