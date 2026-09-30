using Ogma3.Services;

namespace Ogma3.Tests.Services;

public sealed class SystemResourceMonitorTest
{
	[Test]
	public async Task ParseMemoryInfoReturnsUsedAndAvailableBytes()
	{
		var memory = SystemResourceMonitor.ParseMemoryInfo("MemTotal: 1024 kB\nMemAvailable: 256 kB\n");

		await Assert.That(memory).IsEqualTo(new MemoryUsage(1_048_576, 262_144));
		await Assert.That(memory?.UsedBytes).IsEqualTo(786_432);
	}

	[Test]
	public async Task ParseMemoryInfoReturnsNullWhenAvailableMemoryIsMissing()
	{
		var memory = SystemResourceMonitor.ParseMemoryInfo("MemTotal: 1024 kB\n");

		await Assert.That(memory).IsNull();
	}

	[Test]
	public async Task ParseCpuTimesCalculatesUsageExcludingIdleTicks()
	{
		var before = SystemResourceMonitor.ParseCpuTimes("cpu 100 0 0 100 0 0 0 0 100 0\n");
		var after = SystemResourceMonitor.ParseCpuTimes("cpu 150 0 0 150 0 0 0 0 200 0\n");

		await Assert.That(SystemResourceMonitor.CalculateCpuUsage(before, after)).IsEqualTo(50d);
	}

	[Test]
	public async Task CalculateCpuUsageReturnsNullWhenCountersDoNotAdvance()
	{
		var times = new CpuTimes(100, 80);

		await Assert.That(SystemResourceMonitor.CalculateCpuUsage(times, times)).IsNull();
	}

	[Test]
	public async Task ParseGarnetPhysicalMemoryReadsGarnetInfoMemoryOutput()
	{
		const string info = """
		                    # Memory
		                    proc_private_memory_size:473251840
		                    proc_physical_memory_size:1252577280
		                    store_index_size:402656256
		                    """;

		await Assert.That(SystemResourceMonitor.ParseGarnetPhysicalMemory(info)).IsEqualTo(1_252_577_280);
	}

	[Test]
	public async Task ParseGarnetPhysicalMemoryReturnsNullWhenFieldIsMissing()
	{
		await Assert.That(SystemResourceMonitor.ParseGarnetPhysicalMemory("store_index_size:402656256")).IsNull();
	}
}
