using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config;
using Ogma3.Services.UserService;

namespace Ogma3.Api;

using Response = Results<Ok, UnauthorizedHttpResult>;

[Handler]
[MapGet("/health")]
public sealed partial class Health(IOptions<Workers> config, IUserService userService, IHttpContextAccessor accessor)
{
	public sealed record Query;

	private readonly HttpContext _context =
		accessor.HttpContext ?? throw new InvalidOperationException("HttpContextAccessor returned null HttpContext");

	private ValueTask<Response> Handle(Query _, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		_context.Response.ContentType = "text/plain";

		try
		{
			var res = Respond();
			return ValueTask.FromResult(res);
		}
		catch (Exception exception)
		{
			return ValueTask.FromException<Response>(exception);
		}
	}

	private Response Respond()
	{
		if (userService.IsSignedIn())
		{
			return TypedResults.Ok();
		}

		var signature = _context.Request.Headers["X-API-KEY"].ToString();

		return signature.Equals(config.Value.StatmonPingSignatureKey)
				? TypedResults.Ok()
				: TypedResults.Unauthorized();
	}
}