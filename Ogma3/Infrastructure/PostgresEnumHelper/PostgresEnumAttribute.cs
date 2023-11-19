#nullable enable


using System;

namespace Ogma3.Infrastructure.PostgresEnumHelper;

[AttributeUsage(AttributeTargets.Enum)]
public class PostgresEnumAttribute(string? name = null) : Attribute
{
	public string? Name { get; } = name;

	public PostgresEnumAttribute() : this(null)
	{
	}
}