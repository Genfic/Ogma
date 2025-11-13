using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public sealed partial class TestController(OgmaUserManager userManager)
{
	public record Query(string Pwd, long Id);

	private async ValueTask<Results<Ok<string>, NotFound>> HandleAsync(Query q, CancellationToken ct)
	{
		// var user = await userManager.FindByIdAsync(q.Id.ToString());
		//
		// if (user is null)
		// {
		// 	return TypedResults.NotFound();
		// }
		//
		// var token = await userManager.GeneratePasswordResetTokenAsync(user);
		// var result = await userManager.ResetPasswordAsync(user, token, q.Pwd);
		//
		// return TypedResults.Ok(result.Succeeded ? "Success" : string.Join(", ", result.Errors.Select(e => e.Description)));

		await Task.Delay(10, ct);
		return Random.Shared.Next(0, 100) > 50 ? TypedResults.NotFound() : TypedResults.Ok("Success");
	}
}