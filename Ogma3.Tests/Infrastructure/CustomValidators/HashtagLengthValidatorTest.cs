using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public class HashtagLengthValidatorTest
{
	private readonly HashtagLengthValidator<int> _validator = new(10);

	[Theory]
	[InlineData("short,short")]
	[InlineData("1234567890, 1234567890")]
	[InlineData("1234567890,      1234567890")]
	public void TestIsValid_Valid(string value)
	{
		var result = _validator.IsValid(value);
		Assert.True(result);
	}
	
	[Theory]
	[InlineData("super long tag,short,super long tag")]
	[InlineData("short, super long tag, super long tag")]
	[InlineData("0123456789a, short")]
	[InlineData("sho       rt, short      ")]
	public void TestIsValid_Invalid(string value)
	{
		var result = _validator.IsValid(value);
		Assert.False(result);
	}
}