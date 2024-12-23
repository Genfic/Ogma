using System.Threading.Tasks;

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
}