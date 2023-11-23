using Xunit;

namespace Utils.Tests.Utils;

public class UnitConvertersTest
{
	[Fact]
	public void TestBytes() =>
		Assert.Equal("100 B", UnitConverters.SizeSuffix(100L, 0));

	[Fact]
	public void TestKilobytes() =>
		Assert.Equal("10 KB", UnitConverters.SizeSuffix(1024L * 10, 0));

	[Fact]
	public void TestMegabytes() =>
		Assert.Equal("10 MB", UnitConverters.SizeSuffix(1024L * 1024 * 10, 0));

	[Fact]
	public void TestGigabytes() =>
		Assert.Equal("10 GB", UnitConverters.SizeSuffix(1024L * 1024 * 1024 * 10, 0));

	[Fact]
	public void TestTerabytes() =>
		Assert.Equal("10 TB", UnitConverters.SizeSuffix(1024L * 1024 * 1024 * 1024 * 10, 0));
}