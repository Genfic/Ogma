using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Tests.Infrastructure.Attributes;

public sealed class MaxFileSizeAttributeTest
{
	private readonly MaxFileSizeAttribute _attribute = new(10_000_000);

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_attribute.GetValidationResult(null, MakeContext())).IsEqualTo(ValidationResult.Success);
	}

	[Test]
	public async Task TestIsValid_FileWithinLimit()
	{
		var file = MakeFile(length: 5_000_000);
		await Assert.That(_attribute.GetValidationResult(file, MakeContext())).IsEqualTo(ValidationResult.Success);
	}

	[Test]
	public async Task TestIsValid_FileAtLimit()
	{
		var file = MakeFile(length: 10_000_000);
		await Assert.That(_attribute.GetValidationResult(file, MakeContext())).IsEqualTo(ValidationResult.Success);
	}

	[Test]
	public async Task TestIsValid_FileOverLimit()
	{
		var file = MakeFile(length: 20_000_000);
		var result = _attribute.GetValidationResult(file, MakeContext());
		await Assert.That(result).IsNotNull();
		await Assert.That(result?.ErrorMessage).IsEqualTo("Maximum allowed file size is 9.54 MB");
	}

	[Test]
	public async Task TestIsValid_NotAFile()
	{
		var result = _attribute.GetValidationResult("definitely not a file", MakeContext());
		await Assert.That(result).IsNotNull();
		await Assert.That(result?.ErrorMessage).IsEqualTo("Object is not a valid file.");
	}

	[Test]
	public async Task TestAddValidation_AddsClientAttributes()
	{
		var context = MakeClientValidationContext();
		_attribute.AddValidation(context);

		await Assert.That(context.Attributes["data-val"]).IsEqualTo("true");
		await Assert.That(context.Attributes["data-val-filesize-max"]).IsEqualTo("10000000");
		await Assert.That(context.Attributes["data-val-filesize"]).IsEqualTo("Maximum allowed file size is 9.54 MB");
	}

	private static ValidationContext MakeContext() => new("instance");

	private static IFormFile MakeFile(long length)
		=> new FormFile(Stream.Null, 0, length, "file", "photo.jpg");

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