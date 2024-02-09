using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services.UserService;

public class UserService(IHttpContextAccessor? accessor) : IUserService
{
	public ClaimsPrincipal? User => accessor?.HttpContext?.User;
}