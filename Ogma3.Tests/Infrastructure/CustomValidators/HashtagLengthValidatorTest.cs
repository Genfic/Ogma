using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class HashtagLengthValidatorTest
{
	private readonly HashtagLengthValidator<int> _validator = new(10);

	[Test]
	[Arguments("short,short")]
	[Arguments("1234567890, 1234567890")]
	[Arguments("1234567890,      1234567890")]
	public async Task TestIsValid_Valid(string value)
	{
		await Assert.That(_validator.IsValid(value)).IsTrue();
	}
	
	[Test]
	[Arguments("super long tag,short,super long tag")]
	[Arguments("short, super long tag, super long tag")]
	[Arguments("0123456789a, short")]
	[Arguments("sho       rt, short      ")]
	public async Task TestIsValid_Invalid(string value)
	{
		await Assert.That(_validator.IsValid(value)).IsFalse();
	}
}