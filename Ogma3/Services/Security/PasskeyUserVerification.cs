using System.Buffers;
using System.Text.Json;

namespace Ogma3.Services.Security;

/// <summary>
/// Server-side enforcement of WebAuthn user verification for passkey assertions.
///
/// Identity's passkey handler only requires the UV bit when
/// <see cref="Microsoft.AspNetCore.Identity.IdentityPasskeyOptions.UserVerificationRequirement"/>
/// is set to "required" globally, which this app deliberately does not do for ordinary sign-in.
/// Reauthentication always requires user verification, so the flag is checked here directly
/// against the authenticator data that was covered by the (already server-verified) signature.
/// </summary>
public static class PasskeyUserVerification
{
	private const int FlagsOffset = 32;
	private const byte UserVerifiedFlag = 0b0000_0100;

	/// <summary>
	/// Extracts the authenticator data bytes from a WebAuthn assertion credential JSON
	/// (as returned by <c>navigator.credentials.get()</c> and serialized by the client).
	/// </summary>
	public static bool IsUserVerified(string assertionJson)
	{
		try
		{
			using var doc = JsonDocument.Parse(assertionJson);
			if (!doc.RootElement.TryGetProperty("response", out var response) ||
				!response.TryGetProperty("authenticatorData", out var authDataElement) ||
				authDataElement.ValueKind != JsonValueKind.String)
			{
				return false;
			}

			return IsUserVerifiedFromBase64Url(authDataElement.GetString()!);
		}
		catch (JsonException)
		{
			return false;
		}
	}

	public static bool IsUserVerifiedFromBase64Url(string base64Url)
	{
		var padded = base64Url.AsSpan();
		var padLength = (4 - padded.Length % 4) % 4;

		Span<char> buffer = stackalloc char[512];
		if (padded.Length + padLength > buffer.Length) return false;

		padded.CopyTo(buffer);
		for (var i = 0; i < padLength; i++) buffer[padded.Length + i] = '=';

		Span<byte> bytes = stackalloc byte[512];
		if (System.Buffers.Text.Base64.DecodeFromChars(buffer[..(padded.Length + padLength)], bytes, out var written, out _) !=
			OperationStatus.Done)
		{
			return false;
		}

		return IsUserVerified(bytes[..written]);
	}

	/// <param name="authenticatorData">Raw authenticator data; at least 37 bytes (32 rpIdHash + 1 flags + 4 sign count).</param>
	public static bool IsUserVerified(ReadOnlySpan<byte> authenticatorData)
		=> authenticatorData.Length > FlagsOffset &&
		   (authenticatorData[FlagsOffset] & UserVerifiedFlag) != 0;
}
