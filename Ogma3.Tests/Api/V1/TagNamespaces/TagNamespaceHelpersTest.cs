using FluentValidation;
using Ogma3.Api.V1.TagNamespaces;
using Ogma3.Data;

namespace Ogma3.Tests.Api.V1.TagNamespaces;

public sealed class TagNamespaceHelpersTest
{
	private static string NameOfLength(int length) => new('a', length);

	[Test]
	[Arguments("Sci-Fi", "sci-fi")]
	[Arguments("Sci Fi", "sci-fi")]
	[Arguments("  Trimmed  ", "trimmed")]
	[Arguments("A & B", "a-b")]
	[Arguments("SCIFI", "scifi")]
	[Arguments("!!!", "")]
	public async Task TestNormalizeSlug(string input, string expected)
	{
		await Assert.That(TagNamespaceHelpers.NormalizeSlug(input)).IsEqualTo(expected);
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("   ")]
	public async Task TestNormalizeAliasNullishBecomesNull(string? input)
	{
		await Assert.That(TagNamespaceHelpers.NormalizeAlias(input)).IsNull();
	}

	[Test]
	[Arguments("NR", "nr")]
	[Arguments("  nr  ", "nr")]
	[Arguments("nR", "nr")]
	public async Task TestNormalizeAlias(string input, string expected)
	{
		await Assert.That(TagNamespaceHelpers.NormalizeAlias(input)).IsEqualTo(expected);
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("   ")]
	public async Task TestNormalizeColorNullishBecomesNull(string? input)
	{
		await Assert.That(TagNamespaceHelpers.NormalizeColor(input)).IsNull();
	}

	[Test]
	[Arguments("#AABBCC", "aabbcc")]
	[Arguments("AABBCC", "aabbcc")]
	[Arguments("  #AbC  ", "abc")]
	public async Task TestNormalizeColorStripsHashAndLowercases(string input, string expected)
	{
		await Assert.That(TagNamespaceHelpers.NormalizeColor(input)).IsEqualTo(expected);
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("   ")]
	public async Task TestNormalizeDescriptionNullishBecomesNull(string? input)
	{
		await Assert.That(TagNamespaceHelpers.TrimToNull(input)).IsNull();
	}

	[Test]
	[Arguments("  padded  ", "padded")]
	public async Task TestNormalizeDescriptionTrims(string input, string expected)
	{
		await Assert.That(TagNamespaceHelpers.TrimToNull(input)).IsEqualTo(expected);
	}

	[Test]
	[Arguments(null)]
	[Arguments("abc")]
	[Arguments("aabbcc")]
	public async Task TestEnsureNormalizedFieldsAcceptsValidInput(string? color)
	{
		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields("Valid Name", "valid-slug", color))
			.ThrowsNothing();
	}

	[Test]
	public async Task TestEnsureNormalizedFieldsRejectsShortName()
	{
		var name = NameOfLength(CTConfig.TagNamespace.MinNameLength - 1);

		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields(name, "valid-slug", null))
			.Throws<ValidationException>();
	}

	[Test]
	public async Task TestEnsureNormalizedFieldsRejectsLongName()
	{
		var name = NameOfLength(CTConfig.TagNamespace.MaxNameLength + 1);

		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields(name, "valid-slug", null))
			.Throws<ValidationException>();
	}

	[Test]
	public async Task TestEnsureNormalizedFieldsRejectsShortSlug()
	{
		// A slug friendlifies down to a single character, which passes the attribute-level minimum only because the
		// attribute ran before normalization.
		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields("Valid Name", "a", null))
			.Throws<ValidationException>();
	}

	[Test]
	public async Task TestEnsureNormalizedFieldsRejectsEmptySlug()
	{
		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields("Valid Name", "", null))
			.Throws<ValidationException>();
	}

	[Test]
	public async Task TestEnsureNormalizedFieldsRejectsLongSlug()
	{
		var slug = NameOfLength(CTConfig.TagNamespace.MaxSlugLength + 1);

		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields("Valid Name", slug, null))
			.Throws<ValidationException>();
	}

	[Test]
	[Arguments("abcde")]
	[Arguments("abcdefg")]
	[Arguments("aabbccddee")]
	[Arguments("ggg")]
	[Arguments("#aabbcc")]
	[Arguments("aabbcc ")]
	public async Task TestEnsureNormalizedFieldsRejectsInvalidColor(string color)
	{
		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields("Valid Name", "valid-slug", color))
			.Throws<ValidationException>();
	}

	[Test]
	[Arguments("abc")]
	[Arguments("abcd")]
	[Arguments("aabbcc")]
	[Arguments("aabbccdd")]
	public async Task TestEnsureNormalizedFieldsAcceptsShortAndAlphaColors(string color)
	{
		await Assert
			.That(() => TagNamespaceHelpers.EnsureNormalizedFields("Valid Name", "valid-slug", color))
			.ThrowsNothing();
	}
}