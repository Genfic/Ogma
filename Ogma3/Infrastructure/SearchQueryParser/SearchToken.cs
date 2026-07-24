using Ogma3.Data.Stories;

namespace Ogma3.Infrastructure.SearchQueryParser;

public abstract record SearchToken;

public sealed record TitleToken(string Value): SearchToken;

public sealed record TagToken(string? Namespace, string Value, bool Negated = false) : SearchToken
{
	public string FullName => Namespace is null
		? Value
		: $"{Namespace}:{Value}".ToLowerInvariant();
}

public sealed record AuthorToken(string Value, bool Negated = false): SearchToken;

public sealed record StatusToken(string Value, bool Negated = false) : SearchToken
{
	public EStoryStatus? Status =>
		EStoryStatusExtensions.TryParse(Value, out var val, true, true) ? val : null;
}

public sealed record RatingToken(string Value, bool Negated = false) : SearchToken;