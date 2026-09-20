using Immediate.Injections.Shared;
using MemoryPack;
using Microsoft.AspNetCore.DataProtection;

namespace Ogma3.Services.DeletionTokenService;

[RegisterTransient<IDeletionTokenService>]
public sealed class DeletionTokenService(IDataProtectionProvider dataProtection) : IDeletionTokenService
{
	private readonly IDataProtector _protector = dataProtection.CreateProtector("DeletionToken.v1");

	public string GenerateToken(long contentId, DateTimeOffset scheduledFor, string contentType)
	{
		var token = new Token(contentId, scheduledFor, contentType);
		var packed = MemoryPackSerializer.Serialize(token);
		var bytes = _protector.Protect(packed);
		return Convert.ToBase64String(bytes);
	}

	public bool TryParseToken(string token, out long contentId, out DateTimeOffset scheduledFor, out string contentType)
	{
		contentId = 0;
		scheduledFor = default;
		contentType = "";

		try
		{
			var bytes = Convert.FromBase64String(token);
			var decrypted = _protector.Unprotect(bytes);
			var unpacked = MemoryPackSerializer.Deserialize<Token>(decrypted);

			if (unpacked is null)
			{
				return false;
			}

			contentId = unpacked.ContentId;
			scheduledFor = unpacked.ScheduledFor;
			contentType = unpacked.ContentType;

			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}

[MemoryPackable]
public sealed partial record Token(long ContentId, DateTimeOffset ScheduledFor, string ContentType);

public interface IDeletionTokenService
{
	string GenerateToken(long contentId, DateTimeOffset scheduledFor, string contentType);
	bool TryParseToken(string token, out long contentId, out DateTimeOffset scheduledFor, out string contentType);
}