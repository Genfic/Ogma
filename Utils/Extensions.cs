using System;
using System.Drawing;
using System.Globalization;
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
        /// Replace non-alphanumeric characters with underscores, and double underscores with single ones.
        /// </summary>
        /// <param name="input">String to friendlify</param>
        /// <returns>Friendlified string</returns>
        public static string Friendlify(this string input)
        {
            var str = new Regex("[^a-zA-Z0-9]").Replace(input, "_");
            str = new Regex("_+").Replace(str, "_");

            return str.ToLower().Trim('_');
        }

        /// <summary>
        /// Convert `System.Drawing.Color` to `#RRGGBB` notation string.
        /// </summary>
        /// <param name="input">Color object to convert</param>
        /// <returns>Resulting string</returns>
        public static string ToHexCss(this Color input)
        {
            string r = input.R.ToString("X"),
                   g = input.G.ToString("X"),
                   b = input.B.ToString("X");

            return "#" + r + g + b;
        }

        /// <summary>
        /// Convert `System.Drawing.Color` to `RRR, GGG, BBB, A.##` notation string.
        /// </summary>
        /// <param name="input">Color to convert</param>
        /// <returns>Resulting string</returns>
        public static string ToCommaSeparatedCss(this Color input)
        {
            var alpha = ((double) input.A).Normalize(0, 255).ToString("F", CultureInfo.InvariantCulture);//.Replace(',', '.');
            return $"{input.R}, {input.G}, {input.B}, {alpha}";
        }
    }
}