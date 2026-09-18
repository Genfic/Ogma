using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Tests.Infrastructure.Attributes;

public sealed class AllowedExtensionsAttributeTest
{
	private readonly AllowedExtensionsAttribute _attribute = new([".jpg", ".png"]);

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_attribute.GetValidationResult(null, MakeContext())).IsEqualTo(ValidationResult.Success);
	}

	[Test]
	[Arguments("photo.jpg", "image/jpeg")]
	[Arguments("photo.png", "image/png")]
	public async Task TestIsValid_Allowed(string fileName, string contentType)
	{
		await Assert.That(_attribute.GetValidationResult(MakeFile(fileName, contentType), MakeContext())).IsEqualTo(ValidationResult.Success);
	}

	[Test]
	[Arguments("photo.gif", "image/gif")]
	[Arguments("photo.jpg", "text/html")]
	[Arguments("photo.txt", "image/png")]
	public async Task TestIsValid_Disallowed(string fileName, string contentType)
	{
		var result = _attribute.GetValidationResult(MakeFile(fileName, contentType), MakeContext());
		await Assert.That(result).IsNotNull();
		await Assert.That(result?.ErrorMessage).IsEqualTo("The only allowed extensions are: .jpg, .png");
	}

	[Test]
	public async Task TestIsValid_NotAFile()
	{
		var result = _attribute.GetValidationResult(123, MakeContext());
		await Assert.That(result).IsNotNull();
		await Assert.That(result?.ErrorMessage).IsEqualTo("Object is not a valid file.");
	}

	[Test]
	public async Task TestAddValidation_AddsClientAttributes()
	{
		var context = MakeClientValidationContext();
		_attribute.AddValidation(context);

		await Assert.That(context.Attributes["data-val-fileextensions"]).IsEqualTo("The only allowed extensions are: .jpg, .png");
		await Assert.That(context.Attributes["data-val-fileextensions-extensions"]).IsEqualTo(".jpg,.png");
		await Assert.That(context.Attributes["accept"]).IsEqualTo(".jpg,.png");
	}

	private static ValidationContext MakeContext() => new("instance");

	private static IFormFile MakeFile(string fileName, string contentType)
	{
		var file = new FormFile(Stream.Null, 0, 0, "file", fileName);
		file.Headers = new HeaderDictionary { ["Content-Type"] = contentType };
		return file;
	}

	private static ClientModelValidationContext MakeClientValidationContext()
	{
		var provider = new EmptyModelMetadataProvider();
		var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
		return new ClientModelValidationContext(
			actionContext,
			provider.GetMetadataForType(typeof(IFormFile)),
			provider,
			new Dictionary<string, string>());
	}
}