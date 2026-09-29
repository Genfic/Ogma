using System.Security.Claims;
using Ogma3.Data.Users;

namespace Ogma3.Services.UserService;

public interface IUserService
{
	ClaimsPrincipal? User { get; }
	long? UserId { get; }
	Task<UserCreationResult> CreateAsync(string username, string email, string password, bool activated = false, string? inviteCode = null, CancellationToken cancellationToken = default);
	Task<UserCreationResult> CreateAsync(OgmaUser user, string password, string? inviteCode = null, CancellationToken cancellationToken = default);
}