#nullable enable
using System.Security.Claims;

namespace Ogma3.Services.UserService;

public interface IUserService
{
    public ClaimsPrincipal? User { get; }
}