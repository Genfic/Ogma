using Ogma3.Infrastructure.SystemResources;

namespace Ogma3.Tests.Infrastructure.SystemResources;

public sealed class LinuxMetricsParserTest
{
	[Test]
	public async Task ParseMemoryInfoReturnsUsedAndAvailableBytes()
	{
		var memory = LinuxMetricsParser.ParseMemoryInfo("MemTotal: 1024 kB\nMemAvailable: 256 kB\n");

		await Assert.That(memory).IsEqualTo(new MemoryUsage(1_048_576, 262_144));
		await Assert.That(memory?.UsedBytes).IsEqualTo(786_432);
	}

	[Test]
	public async Task ParseMemoryInfoReturnsNullWhenAvailableMemoryIsMissing()
	{
		var memory = LinuxMetricsParser.ParseMemoryInfo("MemTotal: 1024 kB\n");

		await Assert.That(memory).IsNull();
	}

	[Test]
	public async Task ParseCpuTimesCalculatesUsageExcludingIdleTicks()
	{
		var before = LinuxMetricsParser.ParseCpuTimes("cpu 100 0 0 100 0 0 0 0 100 0\n");
		var after = LinuxMetricsParser.ParseCpuTimes("cpu 150 0 0 150 0 0 0 0 200 0\n");

		await Assert.That(LinuxMetricsParser.CalculateCpuUsage(before, after)).IsEqualTo(50d);
	}

	[Test]
	public async Task CalculateCpuUsageReturnsNullWhenCountersDoNotAdvance()
	{
		var times = new CpuTimes(100, 80);

		await Assert.That(LinuxMetricsParser.CalculateCpuUsage(times, times)).IsNull();
	}
}