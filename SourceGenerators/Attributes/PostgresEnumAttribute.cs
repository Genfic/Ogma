#nullable enable
using System;

namespace PostgresEnumHelpers.Generated;

[AttributeUsage(AttributeTargets.Enum)]
public class PostgresEnumAttribute : Attribute
{
    public string? Alias { get; }
}