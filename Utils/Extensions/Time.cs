using System;
using System.Globalization;

namespace Utils.Extensions;

public static class Time
{
	public static string FormatDateWithDaySuffix(this DateTime dateTime)
	{
		var suffix = dateTime.Day.GetOrdinalSuffix();

		return string.Format(dateTime.ToString("d{0} MMMM yyyy", CultureInfo.InvariantCulture), suffix);
	}

	public static string FormatDateWithDaySuffix(this DateTimeOffset dateTime)
	{
		var suffix = dateTime.Day.GetOrdinalSuffix();

		return string.Format(dateTime.ToString("d{0} MMMM yyyy", CultureInfo.InvariantCulture), suffix);
	}
}