using System;

namespace Utils;

public static class UnitConverters
{
	private static readonly string[] SizeSuffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

	public static string SizeSuffix(long value, int decimalPlaces = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(decimalPlaces);

		switch (value)
		{
			case < 0:
				return $"-{SizeSuffix(-value)}";
			case 0:
				return string.Format($"{{0:n{decimalPlaces}}} B", 0);
		}

		// mag is 0 for bytes, 1 for KB, 2, for MB, etc.
		var mag = (int)Math.Log(value, 1024);

		// 1L << (mag * 10) == 2 ^ (10 * mag)
		// [i.e., the number of bytes in the unit corresponding to mag]
		var adjustedSize = (decimal)value / (1L << mag * 10);

		// make adjustment when the value is large enough that
		// it would round up to 1000 or more
		if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
		{
			mag += 1;
			adjustedSize /= 1024;
		}

		return string.Format(
			$"{{0:n{decimalPlaces}}} {{1}}",
			adjustedSize,
			SizeSuffixes[mag]
		);
	}
}