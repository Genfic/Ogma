using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Ogma3.Data.Tags;

[Obsolete(error: false, message: "This enum is obsolete and will be removed in the future.")]
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