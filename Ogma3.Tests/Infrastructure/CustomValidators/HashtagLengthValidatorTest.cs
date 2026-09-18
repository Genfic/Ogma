using FluentValidation;
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

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_validator.IsValid(null)).IsTrue();
	}

	[Test]
	public async Task TestIsValid_WithContext()
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), "short,short")).IsTrue();
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), "0123456789a")).IsFalse();
	}

	[Test]
	public async Task TestName()
	{
		await Assert.That(_validator.Name).IsEqualTo("HashtagLengthValidator");
	}

	private sealed class Dto
	{
		public string? Tags { get; set; }
	}

	[Test]
	public async Task TestErrorMessage_LongTag()
	{
		var validator = new InlineValidator<Dto>();
		validator.RuleFor(d => d.Tags).HashtagsShorterThan(10);

		var result = validator.Validate(new Dto { Tags = "0123456789a" });

		await Assert.That(result.IsValid).IsFalse();
		await Assert.That(result.Errors).IsNotEmpty();
		await Assert.That(result.Errors[0].ErrorMessage).IsEqualTo("No tag can be longer than 10 characters");
	}
}