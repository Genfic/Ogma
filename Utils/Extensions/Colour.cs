using System;
using System.Drawing;
using System.Globalization;

namespace Utils.Extensions;

public static class Colour
{
	/// <summary>
	/// Convert `System.Drawing.Color` to `#RRGGBB` notation string.
	/// </summary>
	/// <param name="color">Color object to convert</param>
	/// <returns>Resulting string</returns>
	public static string ToHexCss(this Color color)
	{
		var hex = (color.R << 16) + (color.G << 8) + color.B;
		return $"#{hex:X6}";
	}

	/// <summary>
	/// Parse hex notation color string into a `System.Drawing.Color` object
	/// </summary>
	/// <param name="input">Input string to parse</param>
	/// <returns>Resulting Color object</returns>
	public static Color ParseHexColor(this string input)
	{
		var hex = input.Trim('#');
		if (!int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var num))
		{
			throw new ArgumentException("Not a valid hexadecimal number", nameof(input));
		}
		
		return hex.Length switch
		{
			3 => Color.FromArgb(((num >> 8) & 0xF) * 0xFF / 0xF, ((num >> 4) & 0xF) * 0xFF / 0xF, (num & 0xF) * 0xFF / 0xF),
			4 => Color.FromArgb(((num >> 12) & 0xF) * 0xFF / 0xF,((num >> 8) & 0xF) * 0xFF / 0xF, ((num >> 4) & 0xF) * 0xFF / 0xF, (num & 0xF) * 0xFF / 0xF),
			6 => Color.FromArgb((num >> 16) & 0xFF, (num >> 8) & 0xFF, num & 0xFF),
			8 => Color.FromArgb((num >> 24) & 0xFF, (num >> 16) & 0xFF, (num >> 8) & 0xFF, num & 0xFF),
			_ => throw new ArgumentException("Unknown format", nameof(input))
		};
	}

	/// <summary>
	/// Convert `System.Drawing.Color` to `RRR, GGG, BBB, A.##` notation string.
	/// </summary>
	/// <param name="input">Color to convert</param>
	/// <returns>Resulting string</returns>
	public static string ToCommaSeparatedCss(this Color input)
	{
		var alpha = ((double)input.A).Normalize(0, 255).ToString("F", CultureInfo.InvariantCulture);
		return $"{input.R}, {input.G}, {input.B}, {alpha}";
	}
}