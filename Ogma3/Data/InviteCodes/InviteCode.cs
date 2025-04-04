using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.InviteCodes;

[AutoDbSet]
public sealed class InviteCode : BaseModel
{
	private readonly string _code = null!;

	public string Code
	{
		get => _code;
		init
		{
			NormalizedCode = value.ToUpper();
			_code = value;
		}
	}

	public string NormalizedCode { get; private set; } = null!;

	public OgmaUser? UsedBy { get; set; }
	public long? UsedById { get; set; }
	public OgmaUser IssuedBy { get; set; } = null!;
	public long IssuedById { get; set; }
	public DateTimeOffset? UsedDate { get; set; }
	public DateTimeOffset IssueDate { get; set; }
}