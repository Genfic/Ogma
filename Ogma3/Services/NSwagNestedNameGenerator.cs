using Namotion.Reflection;
using NJsonSchema.Annotations;
using NJsonSchema.Generation;

namespace Ogma3.Services;

public class NSwagNestedNameGenerator : ISchemaNameGenerator
{
	public string Generate(Type type)
	{
		var typeAttribute = type.ToCachedType().GetAttribute<JsonSchemaAttribute>(true);

		if (!string.IsNullOrEmpty(typeAttribute?.Name))
			return typeAttribute.Name;

		var cachedType = type.ToCachedType();

		if (!cachedType.Type.IsConstructedGenericType)
			return GetName(cachedType);

		return GetName(cachedType).Split('`').First() + "Of" + string.Join("And",
			cachedType.GenericArguments.Select((Func<CachedType, string>)(a => Generate(a.OriginalType))));
	}

	private static string GetName(CachedType cType)
	{
		return cType.Name switch
		{
			"Int16" => GetNullableDisplayName(cType, "Short"),
			"Int32" => GetNullableDisplayName(cType, "Integer"),
			"Int64" => GetNullableDisplayName(cType, "Long"),
			_ when cType.Type.IsConstructedGenericType => GetNullableDisplayName(cType, cType.Name),
			_ => GetNullableDisplayName(cType, GetNameWithNamespace(cType)),
		};
	}

	private static string GetNameWithNamespace(CachedType cType)
		=> cType.Type.FullName!.Split('.')[^1].Replace("+", "");

	private static string GetNullableDisplayName(CachedType type, string actual)
		=> (type.IsNullableType ? "Nullable" : "") + actual;
}