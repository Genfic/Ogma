using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Config;
using Ogma3.Services.CodeGenerator;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<BadRequest<string>, Ok<string>>;

[Handler]
[MapGet("api/generate-invite-code")]
public sealed partial class IssueInviteCodeExternal
(
	ApplicationDbContext context,
	ICodeGenerator codeGenerator,
	IOptions<Workers> options)
{
	private readonly string _master = options.Value.DiscordBotSignatureKey;

	private async ValueTask<ReturnType> HandleAsync(
		Command command,
		CancellationToken cancellationToken
	)
	{
		if (!Verify(_master, command.Key))
		{
			return TypedResults.BadRequest("Invalid key");
		}

		var code = new InviteCode
		{
			Code = codeGenerator.GetInviteCode(),
			IssuedByType = "External",
		};
		context.InviteCodes.Add(code);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(code.Code);
	}

	private static bool Verify(string master, string key)
	{
		var current = DeriveKey(master);
		if (current == key)
		{
			return true;
		}

		var past = DeriveKey(master, -1);
		return past == key;
	}

	private static string DeriveKey(string master, int hourOffset = 0)
	{
		var currentHour = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 3600) + hourOffset; // hours
		Span<byte> hourBytes = stackalloc byte[8];
		BinaryPrimitives.WriteInt64BigEndian(hourBytes, currentHour);

		var masterMaxByteCount = Encoding.UTF8.GetMaxByteCount(master.Length);
		var masterBuffer = masterMaxByteCount > 256 // just in case, since stackalloc has size limits
			? new byte[masterMaxByteCount]
			: stackalloc byte[masterMaxByteCount];
		var masterBytesWritten = Encoding.UTF8.GetBytes(master, masterBuffer);

		var ikm = masterBuffer[..masterBytesWritten];

		Span<byte> derivedKey = stackalloc byte[32];

		HKDF.DeriveKey(
			hashAlgorithmName: HashAlgorithmName.SHA256,
			ikm: ikm,
			output: derivedKey,
			salt: ReadOnlySpan<byte>.Empty,
			info: hourBytes
		);

		return Convert.ToBase64String(derivedKey);
	}

	public sealed record Command(string Key);
}