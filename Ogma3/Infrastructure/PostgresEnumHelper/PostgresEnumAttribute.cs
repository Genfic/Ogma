#nullable enable
using System;

namespace Ogma3.Infrastructure.PostgresEnumHelper;

[AttributeUsage(AttributeTargets.Enum)]
public class PostgresEnumAttribute : Attribute
{
    public string? Name { get; }

    public PostgresEnumAttribute(string? name = null) => Name = name;
    public PostgresEnumAttribute() => Name = null;
}