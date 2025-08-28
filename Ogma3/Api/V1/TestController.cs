using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Services.FileLogService;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public sealed partial class TestController(IFileLogService fls)
{
	public record Query(string Msg);

	private async ValueTask<Ok<string>> HandleAsync(Query q, CancellationToken ct)
	{
		await fls.Write(q.Msg, ct);
		var cont = fls.ReadAll();
		return TypedResults.Ok(cont);
	}
}