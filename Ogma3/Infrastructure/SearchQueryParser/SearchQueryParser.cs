using System.Collections.Frozen;
using Utils.Extensions;

namespace Ogma3.Infrastructure.SearchQueryParser;

using LookupDict = FrozenDictionary<string, Func<string, bool, SearchToken>>;
using AliasLookup = Dictionary<string,string>.AlternateLookup<ReadOnlySpan<char>>;

public static class SearchQueryParser
{
	private static readonly LookupDict FieldParsers =
		new Dictionary<string, Func<string, bool, SearchToken>>(StringComparer.OrdinalIgnoreCase)
		{
			["author"] = (val, neg) => new AuthorToken(val, neg),
			["status"] = (val, neg) => new StatusToken(val, neg),
			["rating"] = (val, neg) => new RatingToken(val, neg),
		}.ToFrozenDictionary();

	private static readonly LookupDict.AlternateLookup<ReadOnlySpan<char>> FieldLookup =
		FieldParsers.GetAlternateLookup<ReadOnlySpan<char>>();

	public static IReadOnlyList<SearchToken> Parse(ReadOnlySpan<char> query, Dictionary<string, string> namespaceAliases)
	{
		var tokens = new List<SearchToken>();

		var aliasLookup = namespaceAliases.GetAlternateLookup<ReadOnlySpan<char>>();

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
					TryAddToken(tokens, query[start..i], aliasLookup);
					start = i + 1;
					break;
			}

		}

		// trailing
		TryAddToken(tokens, query[start..], aliasLookup);

		return tokens.AsReadOnly();
	}

	private static void TryAddToken(List<SearchToken> tokens, ReadOnlySpan<char> rawSegment, AliasLookup aliasLookup)
	{
		var segment = rawSegment.Trim();
		if (segment.IsEmpty)
		{
			return;
		}

		tokens.Add(ParseSegment(segment, aliasLookup));
	}


	private static SearchToken ParseSegment(ReadOnlySpan<char> segment, AliasLookup aliasLookup)
	{
		var negated = segment.StartsWith('-');
		segment = negated ? segment[1..] : segment;

		var colon = segment.IndexOf(':');

		// "some title"
		if (segment is ['"', .. var title, '"'])
		{
			return new TitleToken(title.Trim().ToString());
		}

		// plain tag
		if (colon <= 0)
		{
			return new TagToken(null, segment.Trim().ToString(), negated);
		}

		// key:value
		var key = segment[..colon].Trim();
		var val = segment[(colon + 1)..].Trim();

		if (FieldLookup.TryGetValue(key, out var fac))
		{
			return fac(val.ToString(), negated);
		}

		// plain tag with namespace
		var trimmed = segment[..colon].Trim();

		// fast path: no spaces to remove, skip the stackalloc copy entirely
		if (trimmed.IndexOf(' ') < 0)
		{
			var subbed = aliasLookup.TryGetValue(trimmed, out var sub) ? sub : trimmed;
			return new TagToken(subbed.ToString(), segment[(colon + 1)..].Trim().ToString(), negated);
		}

		// slow path: contains spaces, perform the stackalloc strip
		Span<char> buffer = stackalloc char[trimmed.Length];
		var ns = trimmed.RemoveChar(' ', buffer);

		var subbedClean = aliasLookup.TryGetValue(ns, out var subClean) ? subClean : ns;
		return new TagToken(subbedClean.ToString(), segment[(colon + 1)..].Trim().ToString(), negated);
	}
}