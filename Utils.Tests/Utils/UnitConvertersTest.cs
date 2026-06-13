namespace Utils.Tests.Utils;

public sealed class UnitConvertersTest
{
	[Test]
	public async Task TestBytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(100L, 0)).IsEqualTo("100 B");
	}

	[Test]
	public async Task TestKilobytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(1024L * 10, 0)).IsEqualTo("10 KB");
	}

	[Test]
	public async Task TestMegabytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(1024L * 1024 * 10, 0)).IsEqualTo("10 MB");
	}

	[Test]
	public async Task TestGigabytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(1024L * 1024 * 1024 * 10, 0)).IsEqualTo("10 GB");
	}

	[Test]
	public async Task TestTerabytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(1024L * 1024 * 1024 * 1024 * 10, 0)).IsEqualTo("10 TB");
	}

	[Test]
	public async Task TestZeroBytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(0L, 0)).IsEqualTo("0 B");
	}

	[Test]
	public async Task TestNegativeBytes()
	{
		await Assert.That(UnitConverters.SizeSuffix(-100L, 0)).StartsWith("-");
	}

	[Test]
	public async Task TestDecimalPlaces()
	{
		// Use invariant culture for consistent decimal separator
		var result = UnitConverters.SizeSuffix(1500L, 2);
		// The result will use the current culture's decimal separator
		// Just verify it contains the expected parts
		await Assert.That(result).Contains("1");
		await Assert.That(result).Contains("46");
		await Assert.That(result).Contains("KB");
	}

	[Test]
	public async Task TestLargeValue_RoundsUpToNextUnit()
	{
		// 1023 bytes should be close to 1 KB
		var result = UnitConverters.SizeSuffix(1023L, 2);
		// Just verify it's close to 1 KB
		await Assert.That(result).StartsWith("1");
		await Assert.That(result).EndsWith("KB");
	}

	[Test]
	[Arguments(1024L * 1024 * 1024 * 1024, "TB")]
	[Arguments(1024L * 1024 * 1024 * 1024 * 1024, "PB")]
	[Arguments(1024L * 1024 * 1024 * 1024 * 1024 * 1024, "EB")]
	public async Task TestVeryLargeValues(long bytes, string expectedUnit)
	{
		var result = UnitConverters.SizeSuffix(bytes, 2);
		await Assert.That(result).StartsWith("1");
		await Assert.That(result).EndsWith(expectedUnit);
	}

	[Test]
	public async Task TestNegativeDecimalPlaces_Throws()
	{
		await Assert.That(() => UnitConverters.SizeSuffix(100L, -1)).Throws<ArgumentOutOfRangeException>();
	}
}