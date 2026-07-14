using System.Diagnostics;
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

	public static string FormatDateWithRomanMonth(this DateTime dateTime)
	{
		var month = RomanMonth(dateTime.Month);
		return $"{dateTime.Day:00} {month} {dateTime.Year:0000}";
	}

	public static string FormatDateWithRomanMonth(this DateTimeOffset dateTime)
	{
		var month = RomanMonth(dateTime.Month);
		return $"{dateTime.Day:00} {month} {dateTime.Year:0000}";
	}

	private static string RomanMonth(int month) => month switch
	{
		1 => "I",
		2 => "II",
		3 => "III",
		4 => "IV",
		5 => "V",
		6 => "VI",
		7 => "VII",
		8 => "VIII",
		9 => "IX",
		10 => "X",
		11 => "XI",
		12 => "XII",
		_ => throw new UnreachableException("Month outside of 1-12 range"),
	};
}