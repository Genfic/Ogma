using System.Text.Json.Serialization.Metadata;
using Ogma3.Infrastructure.Sqids;

namespace Ogma3.Infrastructure.OpenApi;

public static class NestedSchemaReferenceId
{
	public static string? Fun(JsonTypeInfo info)
	{
		var type = Nullable.GetUnderlyingType(info.Type) ?? info.Type;

		if (PrimitiveTypes.Contains(type) || type.IsArray)
		{
			return null;
		}

		return Generate(info.Type);
	}

	private static string? Generate(Type type)
	{
		if (!type.IsConstructedGenericType)
		{
			return type.FullName?.Split('.')[^1].Replace("+", "");
		}

		return null;
	}

	private static readonly List<Type> PrimitiveTypes =
	[
		typeof(bool),
		typeof(byte),
		typeof(sbyte),
		typeof(byte[]),
		typeof(string),
		typeof(int),
		typeof(uint),
		typeof(nint),
		typeof(nuint),
		typeof(Int128),
		typeof(UInt128),
		typeof(long),
		typeof(ulong),
		typeof(float),
		typeof(double),
		typeof(decimal),
		typeof(Half),
		typeof(ulong),
		typeof(short),
		typeof(ushort),
		typeof(char),
		typeof(object),
		typeof(DateTime),
		typeof(DateTimeOffset),
		typeof(TimeOnly),
		typeof(DateOnly),
		typeof(TimeSpan),
		typeof(Guid),
		typeof(Uri),
		typeof(Version),
		typeof(Sqid), // technically not a primitive, but we want it inlined
	];
}