using System;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Extensions
    {
        public static bool Between<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
        {
            return num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0;
        }

        public static string Friendlify(this string input)
        {
            var rgx = new Regex("[^a-zA-Z0-9 -]");
            var str = rgx.Replace(input, "_");
            
            rgx = new Regex("_+");
            str = rgx.Replace(str, "_");

            return str.ToLower().Trim('_');
        }
    }
}