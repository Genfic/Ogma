using Microsoft.AspNetCore.Http;
using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class FileExtensionAttributeTest
{
    private static IFormFile MakeFormFile(string fileName)
    {
        var stream = new MemoryStream([]);
        return new FormFile(stream, 0, 0, "file", fileName);
    }

    // null is always valid (field is optional)
    [Test]
    public async Task ValidateProperty_ReturnsTrue_WhenValueIsNull()
    {
        var result = FileExtensionAttribute.ValidateProperty(null, [".jpg", ".png"]);
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(".jpg")]
    [Arguments(".JPG")]   // case-insensitive
    [Arguments(".png")]
    [Arguments(".PNG")]
    public async Task ValidateProperty_ReturnsTrue_WhenExtensionIsAllowed(string ext)
    {
        var file = MakeFormFile($"photo{ext}");
        var result = FileExtensionAttribute.ValidateProperty(file, [".jpg", ".png"]);
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(".exe")]
    [Arguments(".sh")]
    [Arguments(".gif")]
    public async Task ValidateProperty_ReturnsFalse_WhenExtensionIsNotAllowed(string ext)
    {
        var file = MakeFormFile($"bad{ext}");
        var result = FileExtensionAttribute.ValidateProperty(file, [".jpg", ".png"]);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task ValidateProperty_ReturnsFalse_WhenFileHasNoExtension()
    {
        var file = MakeFormFile("noextension");
        var result = FileExtensionAttribute.ValidateProperty(file, [".jpg", ".png"]);
        await Assert.That(result).IsFalse();
    }
}
