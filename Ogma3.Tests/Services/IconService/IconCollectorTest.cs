using Ogma3.Services.IconService;

namespace Ogma3.Tests.Services.IconService;

public sealed class IconCollectorTest
{
	[Test]
	public async Task TestRegisterIcon()
	{
		var collector = new IconCollector();
		collector.RegisterIcon("mdi:heart");

		await Assert.That(collector.RequestedIcons).IsEquivalentTo(["mdi:heart"]);
	}

	[Test]
	public async Task TestRegisterIcons()
	{
		var collector = new IconCollector();
		collector.RegisterIcons(["mdi:heart", "mdi:star", "mdi:heart"]);

		await Assert.That(collector.RequestedIcons).IsEquivalentTo(["mdi:heart", "mdi:star"]);
	}

	[Test]
	public async Task TestRegisterIconsMergesWithExisting()
	{
		var collector = new IconCollector();
		collector.RegisterIcon("mdi:heart");
		collector.RegisterIcons(["mdi:star"]);

		await Assert.That(collector.RequestedIcons).IsEquivalentTo(["mdi:heart", "mdi:star"]);
	}

	[Test]
	public async Task TestClear()
	{
		var collector = new IconCollector();
		collector.RegisterIcon("mdi:heart");
		collector.Clear();

		await Assert.That(collector.RequestedIcons).IsEmpty();
	}
}