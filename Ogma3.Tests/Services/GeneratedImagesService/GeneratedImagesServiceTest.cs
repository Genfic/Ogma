using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config;
using Ogma3.Infrastructure.OgmaConfig;
using ImagesService = Ogma3.Services.GeneratedImagesService.GeneratedImagesService;

namespace Ogma3.Tests.Services.GeneratedImagesService;

public sealed class GeneratedImagesServiceTest
{
	private sealed class OptionsSnapshotStub<T>(T value) : IOptionsSnapshot<T>
		where T : class
	{
		public T Value => value;

		public T Get(string? name) => value;
	}

	[Test]
	public async Task TestGenerateAvatarUrl()
	{
		var service = new ImagesService(
			new OptionsSnapshotStub<Workers>(new Workers
			{
				AvatarServiceSignatureKey = "secret",
				DiscordBotSignatureKey = "discord-secret",
			}),
			new OgmaConfig { AvatarServiceUrl = "https://avatars.example.com/avatar" }
		);

		var url = service.GenerateAvatarUrl("Ursula");

		var expected = "https://avatars.example.com/avatar?name=Ursula&sig=" + ExpectedSignature("secret", "Ursula");
		await Assert.That(url).IsEqualTo(expected);
	}

	private static string ExpectedSignature(string key, string name)
	{
		var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(name));
		return Convert.ToHexString(hash).ToLower();
	}
}