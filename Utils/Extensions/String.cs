using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Utils.Extensions;

public static partial class String
{
	/// <param name="input">String to friendlify</param>
	extension(string input)
	{
		/// <summary>
		/// Replace non-alphanumeric characters with underscores, and double underscores with single ones.
		/// </summary>
		/// <param name="separator">Char to use as a replacement for non-alpha characters</param>
		/// <returns>Friendlified string</returns>
		public string Friendlify(char separator)
		{
			return NonAlphanumericCharactersRegex
				.Replace(input, separator.ToString())
				.ToLower()
				.Trim(separator);
		}
		/// <summary>
		/// Because expression trees are fucking garbage piece of shit
		/// </summary>
		/// <returns></returns>
		public string Friendlify() => input.Friendlify('-');

		/// <summary>
		/// Replaces elements of the `template` according to the supplied `pattern`
		/// </summary>
		/// <param name="pattern">Dictionary in which keys are values to be replaced and values are values to replace them with</param>
		/// <returns>Resulting string</returns>
		public string ReplaceWithPattern(Dictionary<string, string> pattern)
		{
			foreach (var (key, value) in pattern)
			{
				input = input.Replace(key, value);
			}

			return input;
		}

		/// <summary>
		/// Replaces elements of the `template` according to the supplied `pattern`
		/// </summary>
		/// <param name="pattern">Dictionary in which keys are values to be replaced and values are values to replace them with</param>
		/// <param name="comparisonType"></param>
		/// <returns>Resulting string</returns>
		public string ReplaceWithPattern(Dictionary<string, string> pattern, StringComparison comparisonType)
		{
			foreach (var (key, value) in pattern)
			{
				input = input.Replace(key, value, comparisonType);
			}

			return input;
		}

		/// <summary>
		/// Removes all leading whitespace
		/// </summary>
		/// <returns>String without leading whitespace</returns>
		public string RemoveLeadingWhiteSpace()
		{
			var lines = input
				.Split([Environment.NewLine], StringSplitOptions.None)
				.Select(s => s.TrimStart(' ', '\t'));
			return string.Join(Environment.NewLine, lines);
		}

		/// <summary>
		/// Truncates a string to the given length and caps it off
		/// </summary>
		/// <param name="length">Truncation length</param>
		/// <param name="cap">String cap, "..." by default</param>
		/// <returns>Truncated and capped string</returns>
		public string Truncate(int length, string cap = "...")
		{
			return input.Length > length
				? input[..length] + cap
				: input;
		}

		public string Trim(int length)
		{
			return input.Length > length
				? input[..length]
				: input;
		}

		/// <summary>
		/// Get the number of words in the given string
		/// </summary>
		/// <returns>Number of words</returns>
		public int Words()
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

		public ImmutableArray<string> FindHashtags()
		{
			return [..HashtagRegex.Matches(input).Select(m => m.Groups["tag"].Value)];
		}
	}


	/// <param name="span">The ReadOnlySpan of characters to process.</param>
	extension(ReadOnlySpan<char> span)
	{
		/// <summary>
		/// Counts the number of lines in the given ReadOnlySpan of characters.
		/// </summary>
		/// <returns>The total number of lines in the provided span.</returns>
		public int CountLines()
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

		public string Capitalize()
		{
			if (span.IsEmpty)
			{
				return string.Empty;
			}

			if (span.Length == 1)
			{
				return char.ToUpper(span[0]).ToString();
			}

			var s = string.Create(span.Length, span, (chars, state) => {
				chars[0] = char.ToUpper(state[0]);
				state[1..].CopyTo(chars[1..]);
			});

			return s;
		}
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

	public record struct Header(byte Level, byte Occurrence, string Body);

	[GeneratedRegex("[^a-zA-Z0-9]+")]
	private static partial Regex NonAlphanumericCharactersRegex { get; }

	[GeneratedRegex(@"(^|\s)(?'tag'#[\w-]{3,})($|\s)")]
	private static partial Regex HashtagRegex { get; }
}