using System;
using System.Globalization;

namespace Utils.Extensions
{
    public static class Time
    {
        public static string FormatDateWithDaySuffix(this DateTime dateTime)
        {
            var suffix = dateTime.Day switch
            {
                1 or 21 or 31 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th"
            };
            var date = dateTime.ToString("d~ MMMM yyyy", CultureInfo.InvariantCulture);
            return date.Replace("~", suffix);
        }
    }
}