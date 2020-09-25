using System;
using System.Globalization;

namespace Utils.Extensions
{
    public static class Time
    {
        public static string FormatDateWithDaySuffix(this DateTime dateTime)
        {
            // string suffix;
            // switch (dateTime.Day)
            // {
            //     case 1:
            //     case 21:
            //     case 31:
            //         suffix = "st";
            //         break;
            //     case 2:
            //     case 22:
            //         suffix = "nd";
            //         break;
            //     case 3:
            //     case 23:
            //         suffix = "rd";
            //         break;
            //     default:
            //         suffix = "th";
            //         break;
            // }
            
            // TODO: Uncomment for C# 9 and .NET 5
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