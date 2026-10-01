using System.Text.RegularExpressions;
using FluentValidation;
using Ogma3.Data;
using Utils.Extensions;

namespace Ogma3.Api.V1.TagNamespaces;

internal static partial class TagNamespaceHelpers
{
	public static string? TrimToNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

	public static string NormalizeSlug(string slug) => slug.Trim().Friendlify('-').ToLowerInvariant();

	public static string? NormalizeAlias(string? alias) => TrimToNull(alias)?.ToLowerInvariant();

	public static string? NormalizeColor(string? color)
	{
		var trimmed = TrimToNull(color);
		return trimmed is null
			? null
			: (trimmed.StartsWith('#') ? trimmed[1..] : trimmed).ToLowerInvariant();
	}

	/// <summary>
	/// The attribute validation runs before normalization, and normalization can empty a value out entirely (a slug of
	/// <c>!!!</c> friendlifies to nothing), so both are re-checked on the normalized form.
	/// </summary>
	public static void EnsureNormalizedFields(string name, string slug, string? color)
	{
		if (name.Length is < CTConfig.TagNamespace.MinNameLength or > CTConfig.TagNamespace.MaxNameLength)
		{
			throw new ValidationException(
				$"Name must be between {CTConfig.TagNamespace.MinNameLength} and {CTConfig.TagNamespace.MaxNameLength} characters long");
		}

		if (slug.Length is < CTConfig.TagNamespace.MinSlugLength or > CTConfig.TagNamespace.MaxSlugLength)
		{
			throw new ValidationException(
				$"Slug must be between {CTConfig.TagNamespace.MinSlugLength} and {CTConfig.TagNamespace.MaxSlugLength} characters long");
		}

		if (color is not null && !HexColorRegex.IsMatch(color))
		{
			throw new ValidationException("Color must be a hex value of three, four, six or eight digits");
		}
	}


	[GeneratedRegex(@"^(?:[0-9a-f]{3,4}|[0-9a-f]{6}|[0-9a-f]{8})$", RegexOptions.CultureInvariant)]
	private static partial Regex HexColorRegex { get; }
}