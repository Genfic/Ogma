using FluentValidation;
using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class LineCountValidatorTest
{
	private readonly LineCountValidator<int> _validator = new(3);

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), null)).IsTrue();
	}

	[Test]
	[Arguments("")]
	[Arguments("single line")]
	[Arguments("two\nlines")]
	[Arguments("two\nlines\n")]
	[Arguments("three\nlines\nhere")]
	[Arguments("three\nlines\n\n")]
	public async Task TestIsValid_Valid(string value)
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), value)).IsTrue();
	}

	[Test]
	[Arguments("four\nlines\nhere\nnow")]
	[Arguments("\n\n\n\n")]
	public async Task TestIsValid_Invalid(string value)
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), value)).IsFalse();
	}

	[Test]
	public async Task TestName()
	{
		await Assert.That(_validator.Name).IsEqualTo("LineCountValidator");
	}

	private sealed class Dto
	{
		public string? Text { get; set; }
	}

	[Test]
	public async Task TestErrorMessage_TooManyLines()
	{
		var validator = new InlineValidator<Dto>();
		validator.RuleFor(d => d.Text).MaximumLines(3);

		var result = validator.Validate(new Dto { Text = "four\nlines\nhere\nnow" });

		await Assert.That(result.IsValid).IsFalse();
		await Assert.That(result.Errors).IsNotEmpty();
		await Assert.That(result.Errors[0].ErrorMessage).IsEqualTo("You can't have more than 3 lines.");
	}
}