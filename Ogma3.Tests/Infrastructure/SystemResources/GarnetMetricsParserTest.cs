using Ogma3.Infrastructure.SystemResources;

namespace Ogma3.Tests.Infrastructure.SystemResources;

public sealed class GarnetMetricsParserTest
{
	[Test]
	public async Task ParseProcessPrivateMemoryReadsGarnetInfoMemoryOutput()
	{
		const string info = """
		                    # Memory
		                    proc_private_memory_size:473251840
		                    proc_physical_memory_size:1252577280
		                    store_index_size:402656256
		                    """;

		await Assert.That(GarnetMetricsParser.ParseProcessPhysicalMemory(info)).IsEqualTo(1_252_577_280);
	}

	[Test]
	public async Task ParseProcessPrivateMemoryReturnsNullWhenFieldIsMissing()
	{
		await Assert.That(GarnetMetricsParser.ParseProcessPhysicalMemory("store_index_size:402656256")).IsNull();
	}
}