using System;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Check whether `num` is greater than `min` and less than `max`.
        /// </summary>
        /// <param name="num">Number to check</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <typeparam name="T">Type implementing IComparable</typeparam>
        /// <returns>True if `num` is between `min` and `max`, false otherwise.</returns>
        public static bool Between<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
        {
            return num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0;
        }
        
        /// <summary>
        /// Remaps the given `num` in range `oldMin` to `oldMax` to new range of `newMin` to `newMax`.
        /// By default, remaps the given `num` to 0...1 range.
        /// </summary>
        /// <param name="num">Number to remap</param>
        /// <param name="oldMin">Original lowest value `num` can take</param>
        /// <param name="oldMax">Original highest value `num` can take</param>
        /// <param name="newMin">New lowest value `num` can take</param>
        /// <param name="newMax">New highest value `num` can take</param>
        /// <returns>Remapped `num`</returns>
        /// <exception cref="ArgumentException">`oldMin` has to be less than `oldMax`, and `newMin` has to be less than `newMax`.</exception>
        public static double Normalize(this double num, double oldMin, double oldMax, double newMin = 0, double newMax = 1)
        {
            if (oldMin >= oldMax || newMin >= newMax) throw new ArgumentException("oldMin has to be less than oldMax, and newMin has to be less than newMax");
            return newMin + (num - oldMin) * (newMax - newMin) / (oldMax - oldMin);
        }
        
        /// <summary>
        /// Clamps the given IComparable between min and max
        /// </summary>
        /// <param name="num">Number to clamp</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <typeparam name="T">Type implementing IComparable</typeparam>
        /// <returns>Value between min and max</returns>
        /// <exception cref="ArgumentException">`min` has to be less than `max`.</exception>
        public static T Clamp<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
        {
            if (min.CompareTo(max) > 0) throw new ArgumentException("min has to be less than max");
            if (num.CompareTo(min) < 0) return min;
            if (num.CompareTo(max) > 0) return max;
            return num;

        }
        
        /// <summary>
        /// Replace non-alphanumeric characters with underscores, and double underscores with single ones.
        /// </summary>
        /// <param name="input">String to friendlify</param>
        /// <returns>Friendlified string</returns>
        public static string Friendlify(this string input)
        {
            var str = new Regex("[^a-zA-Z0-9]").Replace(input, "-");
            str = new Regex("-+").Replace(str, "-");

            return str.ToLower().Trim('-');
        }

        /// <summary>
        /// Convert `System.Drawing.Color` to `#RRGGBB` notation string.
        /// </summary>
        /// <param name="input">Color object to convert</param>
        /// <returns>Resulting string</returns>
        public static string ToHexCss(this Color input)
        {
            string r = input.R.ToString("X2"),
                   g = input.G.ToString("X2"),
                   b = input.B.ToString("X2");

            return "#" + r + g + b;
        }

        /// <summary>
        /// Parse hex notation color string into a `System.Drawing.Color` object
        /// </summary>
        /// <param name="input">Input string to parse</param>
        /// <returns>Resulting Color object</returns>
        public static Color ParseHexColor(this string input)
        {
            var hex = input.Trim('#');
            var values = hex.Length switch
            {
                3 => new[] {"FF", hex[0].ToString(), hex[1].ToString(), hex[2].ToString()},
                4 => new[] {hex[0].ToString(), hex[1].ToString(), hex[2].ToString(), hex[3].ToString()},
                6 => new[] {"FF", hex[..2], hex[2..4], hex[4..6]},
                8 => new[] {hex[..2], hex[2..4], hex[4..6], hex[6..8]},
                _ => throw new ArgumentException("Incorrect format")
            };
            return Color.FromArgb(
                int.Parse(values[0], NumberStyles.HexNumber),
                int.Parse(values[1], NumberStyles.HexNumber),
                int.Parse(values[2], NumberStyles.HexNumber),
                int.Parse(values[3], NumberStyles.HexNumber)
            );
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
}