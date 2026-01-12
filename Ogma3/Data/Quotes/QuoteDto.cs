namespace Ogma3.Data.Quotes;

public sealed record QuoteDto(string Body, string Author);

public sealed record FullQuoteDto(long Id, string Body, string Author);