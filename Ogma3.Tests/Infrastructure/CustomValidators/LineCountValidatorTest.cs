using FluentValidation;
using FluentValidation.TestHelper;
using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class LineCountValidatorTest
{
    // A minimal inline validator that uses the LineCountValidator under test
    private sealed class TestModel
    {
        public string? Content { get; init; }
    }

    private sealed class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator(uint max)
        {
            RuleFor(x => x.Content).MaximumLines(max);
        }
    }

    [Test]
    public async Task IsValid_ReturnsTrue_WhenValueIsNull()
    {
        var validator = new TestValidator(3);
        var result = validator.TestValidate(new TestModel { Content = null });
        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    [Arguments("single line", 1u)]
    [Arguments("line one\nline two", 2u)]
    [Arguments("a\nb\nc", 3u)]
    public async Task IsValid_ReturnsTrue_WhenLineCountWithinMax(string content, uint max)
    {
        var validator = new TestValidator(max);
        var result = validator.TestValidate(new TestModel { Content = content });
        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    [Arguments("a\nb\nc\nd", 3u)]
    [Arguments("1\n2\n3\n4\n5", 4u)]
    public async Task IsValid_ReturnsFalse_WhenLineCountExceedsMax(string content, uint max)
    {
        var validator = new TestValidator(max);
        var result = validator.TestValidate(new TestModel { Content = content });
        await Assert.That(result.IsValid).IsFalse();
    }
}
