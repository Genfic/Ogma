using Ogma3.Services.TimeService;

namespace Ogma3.Tests.Services.TimeService;

public sealed class TimezoneEntryTest
{
	[Test]
	public async Task TestCompareToOrdersByOffset()
	{
		var utc = new TimezoneEntry("UTC", "UTC", TimeSpan.Zero);
		var minusFive = new TimezoneEntry("minus", "minus", TimeSpan.FromHours(-5));
		var plusTwo = new TimezoneEntry("plus", "plus", TimeSpan.FromHours(2));

		await Assert.That(minusFive.CompareTo(utc)).IsLessThan(0);
		await Assert.That(plusTwo.CompareTo(utc)).IsGreaterThan(0);
		await Assert.That(utc.CompareTo(utc)).IsEqualTo(0);
	}

	[Test]
	public async Task TestCompareToNullIsGreaterThan()
	{
		var utc = new TimezoneEntry("UTC", "UTC", TimeSpan.Zero);
		await Assert.That(utc.CompareTo(null)).IsGreaterThan(0);
	}
}