using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config;
using TimeServiceType = Ogma3.Services.TimeService.TimeService;

namespace Ogma3.Tests.Services.TimeService;

public sealed class TimeServiceTest
{
	private static TimeServiceType CreateService(bool builtInStyle) =>
		new(Options.Create(new TimeOptions { UseBuiltInTimezoneStyle = builtInStyle }));

	[Test]
	public async Task TestBuiltInStyleProducesNonEmptyOrderedEntries()
	{
		var service = CreateService(builtInStyle: true);

		await Assert.That(service.AvailableTimezones).IsNotEmpty();
		await Assert.That(service.AvailableTimezones.All(e => e.Value.Length > 0)).IsTrue();

		var ordered = true;
		for (var i = 1; i < service.AvailableTimezones.Length; i++)
		{
			if (service.AvailableTimezones[i - 1].Offset > service.AvailableTimezones[i].Offset)
			{
				ordered = false;
				break;
			}
		}
		await Assert.That(ordered).IsTrue();
	}

	[Test]
	public async Task TestCustomStyleProducesLabelsContainingUtcOffset()
	{
		var service = CreateService(builtInStyle: false);

		await Assert.That(service.AvailableTimezones).IsNotEmpty();
		await Assert.That(service.AvailableTimezones.All(e => e.Text.StartsWith("(UTC"))).IsTrue();
	}
}