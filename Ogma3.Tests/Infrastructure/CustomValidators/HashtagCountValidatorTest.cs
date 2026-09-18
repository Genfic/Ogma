using FluentValidation;
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

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_validator.IsValid(null)).IsTrue();
	}

	[Test]
	[Arguments("a,,b")]
	[Arguments(",,,")]
	public async Task TestIsValid_EmptySegmentsAreIgnored(string value)
	{
		await Assert.That(_validator.IsValid(value)).IsTrue();
	}

	[Test]
	public async Task TestIsValid_WithContext()
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), "a, b")).IsTrue();
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), "a, b, c, d")).IsFalse();
	}

	[Test]
	public async Task TestName()
	{
		await Assert.That(_validator.Name).IsEqualTo("HashtagCountValidator");
	}

	private sealed class Dto
	{
		public string? Tags { get; set; }
	}

	[Test]
	public async Task TestErrorMessage_TooManyTags()
	{
		var validator = new InlineValidator<Dto>();
		validator.RuleFor(d => d.Tags).HashtagsFewerThan(3);

		var result = validator.Validate(new Dto { Tags = "a,b,c,d" });

		await Assert.That(result.IsValid).IsFalse();
		await Assert.That(result.Errors).IsNotEmpty();
		await Assert.That(result.Errors[0].ErrorMessage).IsEqualTo("You can't use more than 3 tags.");
	}
}