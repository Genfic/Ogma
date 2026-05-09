using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config.RemoteSecrets;
using Ogma3.Infrastructure.OgmaConfig;

namespace Ogma3.Services.GeneratedImagesService;

[RegisterScoped]
[UsedImplicitly]
public sealed class GeneratedImagesService(IOptionsSnapshot<Workers> options, OgmaConfig config)
{
	public string GenerateAvatarUrl(string username)
	{
		var keyBytes = Encoding.UTF8.GetBytes(options.Value.AvatarServiceSignatureKey);
		var nameBytes = Encoding.UTF8.GetBytes(username);

		using var hmac = new HMACSHA256(keyBytes);

		var hashBytes = hmac.ComputeHash(nameBytes);

		var sig = Convert.ToHexString(hashBytes).ToLower();
		return $"{config.AvatarServiceUrl}?name={username}&sig={sig}";
	}
}