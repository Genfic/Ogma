using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using NetEscapades.EnumGenerators;
using NpgSqlGenerators;

namespace Ogma3.Data.Tags;

[PostgresEnum]
[EnumExtensions]
public enum ETagNamespace
{
	[Display(Name = "Content Warning")]
	[EnumMember(Value = "cw")]
	ContentWarning = 1,
	[EnumMember(Value = "g")]
	Genre = 2,
	[EnumMember(Value = "f")]
	Franchise = 3,
}

public static partial class ETagNamespaceExtensions
{
	public static string GetColor(this ETagNamespace? ns) => ns switch
	{
		ETagNamespace.ContentWarning => "#d91919",
		ETagNamespace.Genre => "#8c37f4",
		ETagNamespace.Franchise => "#18f900",
		_ => "transparent",
	};
}