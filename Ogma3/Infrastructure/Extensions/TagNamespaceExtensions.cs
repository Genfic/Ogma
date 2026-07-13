using Ogma3.Data.Tags;

namespace Ogma3.Infrastructure.Extensions;

public static class TagNamespaceExtensions
{
	public static string GetColor(this ETagNamespace? ns) => ns switch
	{
		ETagNamespace.ContentWarning => "#d91919",
		ETagNamespace.Genre => "#8c37f4",
		ETagNamespace.Franchise => "#18f900",
		_ => "rgb(150 150 150 / 0.4)",
	};
}