using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Ogma3.Infrastructure.OpenApi;

namespace Ogma3.Tests.Infrastructure.OpenApi;

public sealed class NestedSchemaReferenceIdTest
{
	private static readonly JsonSerializerOptions Options = new()
	{
		TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
	};

	[Test]
	public async Task TestPrimitiveTypeReturnsNull()
	{
		var info = Options.GetTypeInfo<int>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsNull();
	}

	[Test]
	public async Task TestStringTypeReturnsNull()
	{
		var info = Options.GetTypeInfo<string>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsNull();
	}

	[Test]
	public async Task TestNullablePrimitiveReturnsNull()
	{
		var info = Options.GetTypeInfo<int?>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsNull();
	}

	[Test]
	public async Task TestArrayReturnsNull()
	{
		var info = Options.GetTypeInfo<int[]>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsNull();
	}

	[Test]
	public async Task TestPlainClassReturnsLastSegmentOfFullName()
	{
		var info = Options.GetTypeInfo<PlainSampleDto>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsEqualTo("PlainSampleDto");
	}

	[Test]
	public async Task TestNestedClassStripsPlusFromFullName()
	{
		var info = Options.GetTypeInfo<OuterSampleDto.InnerSampleDto>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsEqualTo("OuterSampleDtoInnerSampleDto");
	}

	[Test]
	public async Task TestConstructedGenericReturnsNull()
	{
		var info = Options.GetTypeInfo<List<int>>();
		await Assert.That(NestedSchemaReferenceId.Fun(info)).IsNull();
	}
}

public sealed record PlainSampleDto(int Value);

public sealed record OuterSampleDto
{
	public sealed record InnerSampleDto(int Value);
}