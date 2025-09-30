using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Utils.Extensions;

public static partial class String
{
	/// <summary>
	/// Replace non-alphanumeric characters with underscores, and double underscores with single ones.
	/// </summary>
	/// <param name="input">String to friendlify</param>
	/// <param name="separator">Char to use as a replacement for non-alpha characters</param>
	/// <returns>Friendlified string</returns>
	public static string Friendlify(this string input, char separator)
	{
		return NonAlphanumericCharactersRegex
			.Replace(input, separator.ToString())
			.ToLower()
			.Trim(separator);
	}

	/// <summary>
	/// Because expression trees are fucking garbage piece of shit
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static string Friendlify(this string input) => Friendlify(input, '-');


	/// <summary>
	/// Counts the number of lines in the given ReadOnlySpan of characters.
	/// </summary>
	/// <param name="span">The ReadOnlySpan of characters to process.</param>
	/// <returns>The total number of lines in the provided span.</returns>
	public static int CountLines(this ReadOnlySpan<char> span)
	{
		if (span.IsEmpty) return 0;

		var count = 0;
		var i = 0;

		while (i < span.Length)
		{
			switch (span[i++])
			{
				case '\r':
				{
					count++;
					if (i < span.Length && span[i] == '\n')
					{
						i++; // Skip the `\n` in `\r\n`
					}
					break;
				}
				case '\n':
					count++;
					break;
			}
		}

		// We need to add one more line in case the string does not end in a newline.
		if (span is not [.., '\n' or '\r'])
		{
			count++;
		}

		return count;
	}

	public static string Capitalize(this ReadOnlySpan<char> input)
	{
		if (input.IsEmpty)
		{
			return string.Empty;
		}

		if (input.Length == 1)
		{
			return char.ToUpper(input[0]).ToString();
		}

		var s = string.Create(input.Length, input, (chars, state) => {
			chars[0] = char.ToUpper(state[0]);
			state[1..].CopyTo(chars[1..]);
		});

		return s;
	}

	/// <summary>
	/// Replaces elements of the `template` according to the supplied `pattern`
	/// </summary>
	/// <param name="template">Template to replace values in</param>
	/// <param name="pattern">Dictionary in which keys are values to be replaced and values are values to replace them with</param>
	/// <returns>Resulting string</returns>
	public static string ReplaceWithPattern(this string template, Dictionary<string, string> pattern)
	{
		foreach (var (key, value) in pattern)
		{
			template = template.Replace(key, value);
		}

		return template;
	}


	/// <summary>
	/// Replaces elements of the `template` according to the supplied `pattern`
	/// </summary>
	/// <param name="template">Template to replace values in</param>
	/// <param name="pattern">Dictionary in which keys are values to be replaced and values are values to replace them with</param>
	/// <param name="comparisonType"></param>
	/// <returns>Resulting string</returns>
	public static string ReplaceWithPattern(this string template, Dictionary<string, string> pattern, StringComparison comparisonType)
	{
		foreach (var (key, value) in pattern)
		{
			template = template.Replace(key, value, comparisonType);
		}

		return template;
	}

	/// <summary>
	/// Removes all leading whitespace
	/// </summary>
	/// <param name="input">String to modify</param>
	/// <returns>String without leading whitespace</returns>
	public static string RemoveLeadingWhiteSpace(this string input)
	{
		var lines = input
			.Split([Environment.NewLine], StringSplitOptions.None)
			.Select(s => s.TrimStart(' ', '\t'));
		return string.Join(Environment.NewLine, lines);
	}

	/// <summary>
	/// Truncates a string to the given length and caps it off
	/// </summary>
	/// <param name="input">String to be truncated</param>
	/// <param name="length">Truncation length</param>
	/// <param name="cap">String cap, "..." by default</param>
	/// <returns>Truncated and capped string</returns>
	public static string Truncate(this string input, int length, string cap = "...")
	{
		return input.Length > length
			? input[..length] + cap
			: input;
	}

	public static string Trim(this string input, int length)
	{
		return input.Length > length
			? input[..length]
			: input;
	}

	/// <summary>
	/// Get the number of words in the given string
	/// </summary>
	/// <param name="input">String to count words in</param>
	/// <returns>Number of words</returns>
	public static int Words(this string input)
	{
		var wasLetter = false;
		var count = 0;
		foreach (var ch in input.AsSpan())
		{
			if (char.IsWhiteSpace(ch))
			{
				wasLetter = false;
			}
			else
			{
				if (!wasLetter)
				{
					count++;
				}

				wasLetter = true;
			}
		}

		return count;
	}

	/// <summary>
	/// Parses a string containing comma-separated tags (possibly #tags) into an array of said tags.
	/// Each tag in the resulting array is trimmed of any whitespace and # characters.
	/// </summary>
	/// <param name="input">String to parse into tags</param>
	/// <returns>An array of strings representing the tags</returns>
	public static ImmutableArray<string> ParseHashtags(this string? input)
	{
		if (string.IsNullOrWhiteSpace(input)) return [];

		var builder = ImmutableArray.CreateBuilder<string>();
		var seen = new HashSet<string>();
		var span = input.AsSpan();

		foreach (var segment in span.Split(','))
		{
			var trimmed = span[segment].Trim();
			if (trimmed.IsEmpty) continue;

			if (trimmed[0] == '#')
			{
				trimmed = trimmed[1..];
			}

			var processed = trimmed.ToString().Friendlify();
			if (seen.Add(processed))
			{
				builder.Add(processed);
			}
		}

		return builder.ToImmutable();
	}

	public static ImmutableArray<string> FindHashtags(this string input)
	{
		return [..HashtagRegex.Matches(input).Select(m => m.Groups["tag"].Value)];
	}

	public record struct Header(byte Level, byte Occurrence, string Body);

	public static List<Header> GetMarkdownHeaders(this string input)
	{
		var headers = new List<Header>();

		var lines = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		foreach (var line in lines)
		{
			if (!line.StartsWith('#')) continue;

			byte level = 0;
			foreach (var c in line)
			{
				if (c == '#')
				{
					level++;
				}
				else
				{
					break;
				}
			}

			var body = line.TrimStart('#').Trim();

			var latest = headers.Count(h => h.Body == body);

			var occurrence = latest > 0
				? (byte)(latest + 1)
				: (byte)0;

			var header = new Header(level, occurrence, body);
			headers.Add(header);
		}

		return headers;
	}

	[GeneratedRegex("[^a-zA-Z0-9]+")]
	private static partial Regex NonAlphanumericCharactersRegex { get; }

	[GeneratedRegex(@"(^|\s)(?'tag'#[\w-]{3,})($|\s)")]
	private static partial Regex HashtagRegex { get; }
}