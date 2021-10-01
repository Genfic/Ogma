using System;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Utils.Extensions;

public static class Colour
{
    /// <summary>
    /// Convert `System.Drawing.Color` to `#RRGGBB` notation string.
    /// </summary>
    /// <param name="input">Color object to convert</param>
    /// <returns>Resulting string</returns>
    public static string ToHexCss(this Color input)
    {
        var r = input.R.ToString("X2");
        var g = input.G.ToString("X2");
        var b = input.B.ToString("X2");

        return $"#{r}{g}{b}";
    }

    /// <summary>
    /// Parse hex notation color string into a `System.Drawing.Color` object
    /// </summary>
    /// <param name="input">Input string to parse</param>
    /// <returns>Resulting Color object</returns>
    public static Color ParseHexColor(this string input)
    {
        var hex = input.Trim('#');
        var values = (hex.Length switch
        {
            3 => new[]
            {
                "FF", 
                hex[0].ToString(), 
                hex[1].ToString(), 
                hex[2].ToString()
            },
            4 => new[]
            {
                hex[0].ToString(), 
                hex[1].ToString(), 
                hex[2].ToString(), 
                hex[3].ToString()
            },
            6 => new[]
            {
                "FF", 
                hex[..2], 
                hex[2..4], 
                hex[4..6]
            },
            8 => new[]
            {
                hex[..2], 
                hex[2..4], 
                hex[4..6], 
                hex[6..8]
            },
            _ => throw new ArgumentException("Incorrect format")
        }).Select(s => int.Parse(s, NumberStyles.HexNumber)).ToArray();
        return Color.FromArgb(values[0], values[1], values[2], values[3]);
    }

    /// <summary>
    /// Convert `System.Drawing.Color` to `RRR, GGG, BBB, A.##` notation string.
    /// </summary>
    /// <param name="input">Color to convert</param>
    /// <returns>Resulting string</returns>
    public static string ToCommaSeparatedCss(this Color input)
    {
        var alpha = ((double) input.A).Normalize(0, 255).ToString("F", CultureInfo.InvariantCulture);
        return $"{input.R}, {input.G}, {input.B}, {alpha}";
    }
}