using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Users;

namespace Ogma3.Services.UserService;

public sealed class UserCreationResult
{
	private readonly List<IdentityError> _errors = [];

	[MemberNotNullWhen(true, nameof(User))]
	public bool Succeeded { get; private init; }

	public OgmaUser? User { get; private init; }

	public IEnumerable<IdentityError> Errors => _errors;

	public static UserCreationResult Success(OgmaUser user)
	{
		var result = new UserCreationResult { User = user, Succeeded = true };
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