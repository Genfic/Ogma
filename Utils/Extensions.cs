using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Extensions
    {
        public static bool Between<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
        {
            return num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0;
        }
        public static double Normalize(this int num, double oldMin, double oldMax, double newMin = 0, double newMax = 1)
        {
            if (Math.Abs(oldMax - oldMin) < 0.00001 || Math.Abs(newMax - newMin) < 0.00001) throw new ArgumentException();
            return newMin + (num - oldMin) * (newMax - newMin) / (oldMax - oldMin);
        }

        public static string Friendlify(this string input)
        {
            var rgx = new Regex("[^a-zA-Z0-9]");
            var str = rgx.Replace(input, "_");
            
            rgx = new Regex("_+");
            str = rgx.Replace(str, "_");

            return str.ToLower().Trim('_');
        }

        public static string ToHexCss(this Color input)
        {
            var r = input.R.ToString("X");
            var g = input.G.ToString("X");
            var b = input.B.ToString("X");

            return $"#{r}{g}{b}";
        }

        public static string ToCommaSeparatedCss(this Color input)
        {
            return $"{input.R}, {input.G}, {input.B}, {((int)input.A).Normalize(0, 255)}";
        }
    }
}