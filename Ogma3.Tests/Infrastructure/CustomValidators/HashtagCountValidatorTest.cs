using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public class HashtagCountValidatorTest
{
	private readonly HashtagCountValidator<int> _validator = new(3);

	[Theory]
	[InlineData("a, b, c")]
	[InlineData("a,b,c")]
	[InlineData("")]
	[InlineData("a, b")]
	[InlineData("a")]
	public void TestIsValid_Valid(string value)
	{
		var result = _validator.IsValid(value);
		Assert.True(result);
	}
	
	[Theory]
	[InlineData("a, b, c, d")]
	[InlineData("a,b,c,d")]
	public void TestIsValid_Invalid(string value)
	{
		var result = _validator.IsValid(value);
		Assert.False(result);
	}
}