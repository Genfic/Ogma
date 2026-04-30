using System.Security.Cryptography;
using System.Text;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config.RemoteSecrets;

namespace Ogma3.Api.V1;

using ReturnType = Results<Ok<string>, NotFound>;

[Handler]
[MapGet("api/test-two")]
public static partial class TestTwo
{
	public sealed record Query(string Name);

	private static async ValueTask<ReturnType> Handle(Query q, IOptions<Workers> cfg, CancellationToken cancellationToken)
	{
		var keyBytes = Encoding.UTF8.GetBytes(cfg.Value.AvatarServiceSignatureKey);
		var nameBytes = Encoding.UTF8.GetBytes(q.Name);

		using var hmac = new HMACSHA256(keyBytes);

		var hashBytes = hmac.ComputeHash(nameBytes);

		var sig = Convert.ToHexString(hashBytes).ToLower();
		var url = $"https://avatars.genfic.net?name={q.Name}&sig={sig}";

		return TypedResults.Ok(url);
	}
}