using System;
using System.Globalization;

namespace Utils.Extensions;

public static class Time
{
	public static string FormatDateWithDaySuffix(this DateTime dateTime)
	{
		var suffix = (dateTime.Day % 10) switch
		{
			1 => "st",
			2 => "nd",
			3 => "rd",
			_ => "th",
		};
		return string.Format(dateTime.ToString("d{0} MMMM yyyy", CultureInfo.InvariantCulture), suffix);
	}
}