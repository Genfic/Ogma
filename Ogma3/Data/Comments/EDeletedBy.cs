using System.ComponentModel;
using NpgSqlGenerators;

namespace Ogma3.Data.Comments;

[PostgresEnum]
public enum EDeletedBy
{
	[Description("Comment deleted by its author.")]
	User = 1,

	[Description("Comment deleted by a member of the staff.")]
	Staff = 2
}