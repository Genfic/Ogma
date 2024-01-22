using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utils.Extensions;

public static partial class String
{
	/// <summary>
	/// Replace non-alphanumeric characters with underscores, and double underscores with single ones.
	/// </summary>
	/// <param name="input">String to friendlify</param>
	/// <returns>Friendlified string</returns>
	public static string Friendlify(this string input)
	{
		return MyRegex()
			.Replace(input, "-")
			.ToLower()
			.Trim('-');
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

	/// <summary>
	/// Get the amount of words in the given string
	/// </summary>
	/// <param name="input">String to count words in</param>
	/// <returns>Amount of words</returns>
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
	public static string[] ParseHashtags(this string? input)
	{
		if (string.IsNullOrWhiteSpace(input)) return [];
		
		return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(t => t.Trim('#').Friendlify())
			.Distinct()
			.ToArray();
	}

	public static IEnumerable<Header> GetMarkdownHeaders(this string input)
	{
		var headers = new List<Header>();
		
		var lines = input.Split(Environment.NewLine);
		foreach (var line in lines)
		{
			if (!line.StartsWith('#')) continue;

			var head = new Header();
			foreach (var c in line)
			{
				if (c == '#')
				{
					head.Level++;
				}
				else
				{
					break;
				}
			}

			head.Body = line.TrimStart('#').Trim();

			var latest = headers
				.Where(h => h.Body == head.Body)
				.MaxBy(h => h.Occurrence);

			if (latest != null)
			{
				head.Occurrence = (byte)(latest.Occurrence + 1);
			}

			headers.Add(head);
		}

		return headers;
	}

	public record Header
	{
		public byte Level { get; set; }
		public byte Occurrence { get; set; }
		public string Body { get; set; } = null!;
	}

	[GeneratedRegex("[^a-zA-Z0-9]+")]
	private static partial Regex MyRegex();
}