using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class FutureAttributeTest
{
    [Test]
    public async Task ValidateProperty_ReturnsTrue_WhenDateIsInFuture()
    {
        var future = DateTimeOffset.UtcNow.AddDays(1);
        await Assert.That(FutureAttribute.ValidateProperty(future)).IsTrue();
    }

    [Test]
    public async Task ValidateProperty_ReturnsFalse_WhenDateIsInPast()
    {
        var past = DateTimeOffset.UtcNow.AddDays(-1);
        await Assert.That(FutureAttribute.ValidateProperty(past)).IsFalse();
    }

    [Test]
    public async Task ValidateProperty_ReturnsFalse_WhenDateIsExactlyNow()
    {
        // DateTimeOffset.UtcNow captured just before the call will always be <= UtcNow inside
        var slightlyPast = DateTimeOffset.UtcNow.AddMilliseconds(-1);
        await Assert.That(FutureAttribute.ValidateProperty(slightlyPast)).IsFalse();
    }

    [Test]
    public async Task ValidateProperty_ReturnsTrue_WhenDateIsFarFuture()
    {
        var farFuture = DateTimeOffset.UtcNow.AddYears(100);
        await Assert.That(FutureAttribute.ValidateProperty(farFuture)).IsTrue();
    }
}
