using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Config;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.CodeGenerator;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<string>>;

[Handler]
[MapGet("api/generate-invite-code")]
[UsedImplicitly]
public sealed partial class IssueInviteCodeExternal
(
	ApplicationDbContext context,
	ICodeGenerator codeGenerator,
	IOptions<Workers> options)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint) => endpoint
		.RequireRateLimiting(RateLimiting.IssueInviteCodeExternal);

	private readonly string _master = options.Value.DiscordBotSignatureKey;

	private async ValueTask<ReturnType> HandleAsync(
		Command command,
		CancellationToken cancellationToken
	)
	{
		if (!Verify(_master, command.Key))
		{
			return TypedResults.Unauthorized();
		}

		var code = new InviteCode
		{
			Code = codeGenerator.GetInviteCode(),
			IssuedByType = command.Issuer ?? "External",
		};
		context.InviteCodes.Add(code);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(code.Code);
	}

	private static bool Verify(string master, string key)
	{
		Span<byte> current = stackalloc byte[32];
		Span<byte> past = stackalloc byte[32];

		DeriveKey(current, master);
		DeriveKey(past, master, -1);

		Span<byte> provided = stackalloc byte[32];
		if (!Convert.TryFromBase64String(key, provided, out var written) || written != 32)
		{
			return false;
		}

		var currentOk = CryptographicOperations.FixedTimeEquals(provided, current);
		var pastOk = CryptographicOperations.FixedTimeEquals(provided, past);

		return currentOk || pastOk;
	}

	private static void DeriveKey(Span<byte> output, string master, int hourOffset = 0)
	{
		var currentHour = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 3600 + hourOffset; // hours
		Span<byte> hourBytes = stackalloc byte[8];
		BinaryPrimitives.WriteInt64BigEndian(hourBytes, currentHour);

		var masterMaxByteCount = Encoding.UTF8.GetMaxByteCount(master.Length);
		var masterBuffer = masterMaxByteCount > 256 // just in case, since stackalloc has size limits
			? new byte[masterMaxByteCount]
			: stackalloc byte[masterMaxByteCount];
		var masterBytesWritten = Encoding.UTF8.GetBytes(master, masterBuffer);

		var ikm = masterBuffer[..masterBytesWritten];

		HKDF.DeriveKey(
			hashAlgorithmName: HashAlgorithmName.SHA256,
			ikm: ikm,
			output: output,
			salt: ReadOnlySpan<byte>.Empty,
			info: hourBytes
		);
	}

	[UsedImplicitly]
	public sealed record Command(
		[FromHeader(Name = "X-Api-Key")] string Key,
		[FromHeader(Name = "X-Issued-By")] string? Issuer
	);
}