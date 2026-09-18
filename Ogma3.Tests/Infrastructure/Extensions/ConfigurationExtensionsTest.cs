using Microsoft.Extensions.Configuration;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Tests.Infrastructure.Extensions;

public sealed class ConfigurationExtensionsTest
{
	[Test]
	public async Task TestRequireReturnsTheConfiguredValue()
	{
		var manager = new ConfigurationManager();
		manager["Answer"] = "42";

		await Assert.That(manager.Require<int>("Answer")).IsEqualTo(42);
	}

	[Test]
	public async Task TestRequireThrowsWhenKeyMissing()
	{
		var manager = new ConfigurationManager();

		await Assert.That(() => manager.Require<string>("Missing")).Throws<InvalidOperationException>();
	}

	[Test]
	public async Task TestRequireThrowsWhenValueIsNull()
	{
		var manager = new ConfigurationManager();
		manager["Empty"] = null!;

		await Assert.That(() => manager.Require<string>("Empty")).Throws<InvalidOperationException>();
	}
}