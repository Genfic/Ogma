using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Users;
using Ogma3.Services.InviteCodeService;

namespace Ogma3.Services.UserService;

public sealed class UserCreationResult
{
	private readonly List<IdentityError> _errors = [];

	[MemberNotNullWhen(true, nameof(User))]
	public bool Succeeded { get; private init; }

	public OgmaUser? User { get; private init; }

	public IEnumerable<IdentityError> Errors => _errors;

	/// <summary>
	/// Why the invite code could not be redeemed, or <see langword="null" /> when the caller did not
	/// require one or the user was rejected before any code was touched.
	/// </summary>
	public InviteCodeRedemptionResult? InviteCode { get; private init; }

	public static UserCreationResult Success(OgmaUser user, InviteCodeRedemptionResult? inviteCode = null)
	{
		var result = new UserCreationResult { User = user, Succeeded = true, InviteCode = inviteCode };
		return result;
	}

	/// <summary>
	/// Rejects the whole creation because the invite code behind it could not be claimed. The
	/// transaction that created the user has been rolled back, so there is nothing to clean up.
	/// </summary>
	public static UserCreationResult Failed(InviteCodeRedemptionResult inviteCode)
	{
		var result = new UserCreationResult { InviteCode = inviteCode };
		return result;
	}

	public static UserCreationResult Failed(params IdentityError[] errors)
	{
		var result = new UserCreationResult { Succeeded = false };
		result._errors.AddRange(errors);
		return result;
	}

	public static UserCreationResult Failed(List<IdentityError> errors)
	{
		var result = new UserCreationResult { Succeeded = false };
		result._errors.AddRange(errors);
		return result;
	}

	public override string ToString()
	{
		return Succeeded ?
			"Succeeded" :
			string.Format(CultureInfo.InvariantCulture, "{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
	}
}