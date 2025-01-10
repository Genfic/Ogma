namespace Ogma3.Data.Quotes;

public record QuoteDto(string Body, string Author);

public record FullQuoteDto(long Id, string Body, string Author);