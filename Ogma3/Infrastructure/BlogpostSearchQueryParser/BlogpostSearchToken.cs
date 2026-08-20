namespace Ogma3.Infrastructure.BlogpostSearchQueryParser;

public abstract record BlogpostSearchToken;

public sealed record BlogpostTitleToken(string Value) : BlogpostSearchToken;

public sealed record BlogpostTagToken(string Value, bool Negated = false) : BlogpostSearchToken;

public sealed record BlogpostAuthorToken(string Value, bool Negated = false) : BlogpostSearchToken;