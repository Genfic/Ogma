using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class TimeTests
{
	[Test]
	[Arguments(0)]
	[Arguments(1)]
	[Arguments(5)]
	[Arguments(9)]
	public async Task MicrosecondEqual_WithinMicrosecond_ReturnsTrue(int ticksOffset)
	{
		var a = new DateTimeOffset(2023, 6, 15, 12, 30, 45, TimeSpan.Zero);
		var b = a.AddTicks(ticksOffset);
		
		await Assert.That(a.MicrosecondEqual(b)).IsTrue();
		await Assert.That(b.MicrosecondEqual(a)).IsTrue();
	}

	[Test]
	[Arguments(10)]
	[Arguments(11)]
	[Arguments(100)]
	[Arguments(10000)]
	public async Task MicrosecondEqual_ExceedsMicrosecond_ReturnsFalse(int ticksOffset)
	{
		var a = new DateTimeOffset(2023, 6, 15, 12, 30, 45, TimeSpan.Zero);
		var b = a.AddTicks(ticksOffset);
		
		await Assert.That(a.MicrosecondEqual(b)).IsFalse();
		await Assert.That(b.MicrosecondEqual(a)).IsFalse();
	}

	[Test]
	public async Task MicrosecondEqual_ExactlyEqual_ReturnsTrue()
	{
		var a = new DateTimeOffset(2023, 6, 15, 12, 30, 45, 123, TimeSpan.Zero);
		var b = new DateTimeOffset(2023, 6, 15, 12, 30, 45, 123, TimeSpan.Zero);
		
		await Assert.That(a.MicrosecondEqual(b)).IsTrue();
	}

	[Test]
	public async Task MicrosecondEqual_DifferentOffsets_SameInstant_ReturnsTrue()
	{
		var a = new DateTimeOffset(2023, 6, 15, 12, 30, 45, TimeSpan.FromHours(2));
		var b = new DateTimeOffset(2023, 6, 15, 10, 30, 45, TimeSpan.Zero);
		
		await Assert.That(a.MicrosecondEqual(b)).IsTrue();
	}

	[Test]
	public async Task MicrosecondEqual_NegativeOffset_ReturnsTrue()
	{
		var a = new DateTimeOffset(2023, 6, 15, 12, 30, 45, TimeSpan.Zero);
		var b = a.AddTicks(-5);
		
		await Assert.That(a.MicrosecondEqual(b)).IsTrue();
		await Assert.That(b.MicrosecondEqual(a)).IsTrue();
	}
	[Test]
	[Arguments(1, "1st")]
	[Arguments(2, "2nd")]
	[Arguments(3, "3rd")]
	[Arguments(4, "4th")]
	[Arguments(21, "21st")]
	[Arguments(22, "22nd")]
	[Arguments(23, "23rd")]
	[Arguments(11, "11th")]
	public async Task FormatDateWithDaySuffix_DateTime(int day, string expectedSuffix)
	{
		var date = new DateTime(2023, 6, day);
		var result = date.FormatDateWithDaySuffix();
		
		var expectedDay = day.ToString();
		await Assert.That(result).Contains(expectedDay);
		await Assert.That(result).Contains(expectedSuffix);
		await Assert.That(result).Contains("June");
		await Assert.That(result).Contains("2023");
	}

	[Test]
	public async Task FormatDateWithDaySuffix_DateTime_11th()
	{
		var date = new DateTime(2023, 6, 11);
		var result = date.FormatDateWithDaySuffix();
		
		await Assert.That(result).Contains("11th");
	}

	[Test]
	public async Task FormatDateWithDaySuffix_DateTime_12th()
	{
		var date = new DateTime(2023, 6, 12);
		var result = date.FormatDateWithDaySuffix();
		
		await Assert.That(result).Contains("12th");
	}

	[Test]
	public async Task FormatDateWithDaySuffix_DateTime_13th()
	{
		var date = new DateTime(2023, 6, 13);
		var result = date.FormatDateWithDaySuffix();
		
		await Assert.That(result).Contains("13th");
	}

	[Test]
	[Arguments(1, "1st")]
	[Arguments(2, "2nd")]
	[Arguments(3, "3rd")]
	[Arguments(4, "4th")]
	public async Task FormatDateWithDaySuffix_DateTimeOffset(int day, string expectedSuffix)
	{
		var date = new DateTimeOffset(2023, 6, day, 0, 0, 0, TimeSpan.Zero);
		var result = date.FormatDateWithDaySuffix();
		
		var expectedDay = day.ToString();
		await Assert.That(result).Contains(expectedDay);
		await Assert.That(result).Contains(expectedSuffix);
		await Assert.That(result).Contains("June");
		await Assert.That(result).Contains("2023");
	}

	[Test]
	public async Task FormatDateWithDaySuffix_DateTimeOffset_11th()
	{
		var date = new DateTimeOffset(2023, 6, 11, 0, 0, 0, TimeSpan.Zero);
		var result = date.FormatDateWithDaySuffix();
		
		await Assert.That(result).Contains("11th");
	}

	[Test]
	[Arguments(1, "I")]
	[Arguments(2, "II")]
	[Arguments(3, "III")]
	[Arguments(4, "IV")]
	[Arguments(5, "V")]
	[Arguments(6, "VI")]
	[Arguments(7, "VII")]
	[Arguments(8, "VIII")]
	[Arguments(9, "IX")]
	[Arguments(10, "X")]
	[Arguments(11, "XI")]
	[Arguments(12, "XII")]
	public async Task FormatDateWithRomanMonth_DateTime(int month, string expectedRoman)
	{
		var date = new DateTime(2023, month, 5);
		var result = date.FormatDateWithRomanMonth();

		await Assert.That(result).IsEqualTo($"05 {expectedRoman} 2023");
	}

	[Test]
	public async Task FormatDateWithRomanMonth_DateTimeOffset()
	{
		var date = new DateTimeOffset(2023, 12, 31, 0, 0, 0, TimeSpan.Zero);
		var result = date.FormatDateWithRomanMonth();

		await Assert.That(result).IsEqualTo("31 XII 2023");
	}
}
