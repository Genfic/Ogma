using System.Security.Claims;
using Ogma3.Data.Users;

namespace Ogma3.Services.UserService;

public interface IUserService
{
	public ClaimsPrincipal? User { get; }
	public long? UserId { get; }
	public Task<UserCreationResult> CreateAsync(string username, string email, string password, bool activated = false);
	public Task<UserCreationResult> CreateAsync(OgmaUser user, string password);
}