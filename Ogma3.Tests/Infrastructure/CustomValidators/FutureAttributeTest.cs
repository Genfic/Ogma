using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class FutureAttributeTest
{
	[Test]
	[Arguments("2099-01-01T00:00:00Z")]
	[Arguments("2050-01-01T00:00:00Z")]
	public async Task TestValidateProperty_Valid(string value)
	{
		await Assert.That(FutureAttribute.ValidateProperty(DateTimeOffset.Parse(value))).IsTrue();
	}

	[Test]
	[Arguments("2020-01-01T00:00:00Z")]
	[Arguments("2010-01-01T00:00:00Z")]
	public async Task TestValidateProperty_Invalid(string value)
	{
		await Assert.That(FutureAttribute.ValidateProperty(DateTimeOffset.Parse(value))).IsFalse();
	}

	[Test]
	public async Task TestDefaultMessage()
	{
		await Assert.That(FutureAttribute.DefaultMessage).IsEqualTo("{PropertyName} must be in the future.");
	}
}