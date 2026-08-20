namespace Ogma3.Infrastructure.BlogpostSearchQueryParser;

public static class BlogpostSearchQueryParser
{
	public static IReadOnlyList<BlogpostSearchToken> Parse(ReadOnlySpan<char> query)
	{
		var tokens = new List<BlogpostSearchToken>();

		var start = 0;
		var quoted = false;

		for (var i = 0; i < query.Length; i++)
		{
			var curr = query[i];
			switch (curr)
			{
				case '"':
					quoted = !quoted;
					continue;
				case ',' when !quoted:
					TryAddToken(tokens, query[start..i]);
					start = i + 1;
					break;
			}
		}

		TryAddToken(tokens, query[start..]);

		return tokens.AsReadOnly();
	}

	private static void TryAddToken(List<BlogpostSearchToken> tokens, ReadOnlySpan<char> rawSegment)
	{
		var segment = rawSegment.Trim();
		if (segment.IsEmpty)
		{
			return;
		}

		tokens.Add(ParseSegment(segment));
	}

	private static BlogpostSearchToken ParseSegment(ReadOnlySpan<char> segment)
	{
		var negated = segment.StartsWith('-');
		segment = negated ? segment[1..] : segment;

		// "some title"
		if (segment is ['"', .. var title, '"'])
		{
			return new BlogpostTitleToken(title.Trim().ToString());
		}

		// author:some-author
		const string authorPrefix = "author:";

		if (segment.StartsWith(authorPrefix, StringComparison.OrdinalIgnoreCase))
		{
			return new BlogpostAuthorToken(segment[authorPrefix.Length..].Trim().ToString());
		}

		// plain tag
		return new BlogpostTagToken(segment.Trim().ToString(), negated);
	}
}