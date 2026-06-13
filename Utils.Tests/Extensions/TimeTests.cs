using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class TimeTests
{
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
}
