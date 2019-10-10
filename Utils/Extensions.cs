using System;

namespace Utils
{
    public static class Extensions
    {
        public static bool Between<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
        {
            return num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0;
        }
    }
}