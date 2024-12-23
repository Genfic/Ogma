using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class HashtagCountValidatorTest
{
	private readonly HashtagCountValidator<int> _validator = new(3);

	[Test]
	[Arguments("a, b, c")]
	[Arguments("a,b,c")]
	[Arguments("")]
	[Arguments("a, b")]
	[Arguments("a")]
	public async Task TestIsValid_Valid(string value)
	{
		await Assert.That(_validator.IsValid(value)).IsTrue();
	}
	
	[Test]
	[Arguments("a, b, c, d")]
	[Arguments("a,b,c,d")]
	public async Task TestIsValid_Invalid(string value)
	{
		await Assert.That(_validator.IsValid(value)).IsFalse();
	}
}