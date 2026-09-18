using FluentValidation;
using Microsoft.AspNetCore.Http;
using Ogma3.Infrastructure.CustomValidators;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class FileExtensionValidatorTest
{
	private readonly FileExtensionValidator<int> _validator = new([".jpg", ".png"]);

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), null)).IsTrue();
	}

	[Test]
	[Arguments("photo.jpg")]
	[Arguments("photo.PNG")]
	[Arguments("DOCS.JPG")]
	public async Task TestIsValid_Valid(string fileName)
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), MakeFile(fileName))).IsTrue();
	}

	[Test]
	[Arguments("photo.gif")]
	[Arguments("photo.jpeg")]
	[Arguments("archive.tar.gz")]
	public async Task TestIsValid_Invalid(string fileName)
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), MakeFile(fileName))).IsFalse();
	}

	[Test]
	public async Task TestValidateProperty_Null()
	{
		await Assert.That(FileExtensionAttribute.ValidateProperty(null, [".jpg", ".png"])).IsTrue();
	}

	[Test]
	[Arguments("photo.jpg")]
	[Arguments("photo.PNG")]
	public async Task TestValidateProperty_Valid(string fileName)
	{
		await Assert.That(FileExtensionAttribute.ValidateProperty(MakeFile(fileName), [".jpg", ".png"])).IsTrue();
	}

	[Test]
	[Arguments("photo.gif")]
	[Arguments("archive.tar.gz")]
	public async Task TestValidateProperty_Invalid(string fileName)
	{
		await Assert.That(FileExtensionAttribute.ValidateProperty(MakeFile(fileName), [".jpg", ".png"])).IsFalse();
	}

	[Test]
	public async Task TestName()
	{
		await Assert.That(_validator.Name).IsEqualTo("FileExtensionValidator");
	}

	[Test]
	public async Task TestAttributeCanBeConstructed()
	{
		var attribute = new FileExtensionAttribute(".jpg", ".png");
		await Assert.That(attribute).IsNotNull();
	}

	[Test]
	public async Task TestDefaultMessage()
	{
		await Assert.That(FileExtensionAttribute.DefaultMessage)
			.IsEqualTo("{PropertyName} must be a file with one of the following extensions: {AllowedExtensionsValue}");
	}

	private sealed class Dto
	{
		public IFormFile? File { get; set; }
	}

	[Test]
	public async Task TestErrorMessage_DisallowedExtension()
	{
		var validator = new InlineValidator<Dto>();
		validator.RuleFor(d => d.File).FileHasExtension(".jpg", ".png");

		var result = validator.Validate(new Dto { File = MakeFile("photo.gif") });

		await Assert.That(result.IsValid).IsFalse();
		await Assert.That(result.Errors).IsNotEmpty();
		await Assert.That(result.Errors[0].ErrorMessage).IsEqualTo("Only allowed extensions are .jpg, .png. Your file is .gif.");
	}

	private static IFormFile? MakeFile(string? fileName)
		=> fileName is null ? null : new FormFile(Stream.Null, 0, 0, "file", fileName);
}