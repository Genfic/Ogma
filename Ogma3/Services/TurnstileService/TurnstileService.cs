using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ogma3.Services.TurnstileService;

public class TurnstileService(
	IHttpClientFactory clientFactory,
	IOptions<TurnstileSettings> options,
	ILogger<TurnstileService> logger) : ITurnstileService
{
	private const string Url = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

	public async Task<TurnstileResult> Verify(string token, string ip)
	{
		if (token.IsNullOrEmpty()) throw new ValidationException("Turnstile token cannot be empty");
		if (ip.IsNullOrEmpty()) throw new ValidationException("IP cannot be empty");
		
		using var client = clientFactory.CreateClient();
		using var formData = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			["secret"] = options.Value.Secret,
			["response"] = token,
			["remoteip"] = ip
		});

		var response = await client.PostAsync(Url, formData);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
		{
			logger.LogError("Turnstile request returned HTTP error {ErrorCode} with message {ErrorMessage}", response.StatusCode, content);
			throw new ValidationException($"Turnstile response returned HTTP code {response.StatusCode}");
		}

		var data = JsonSerializer.Deserialize(content, TurnstileResultContext.Default.TurnstileResult);

		if (data is not null) return data;
		
		logger.LogError("Turnstile response was empty");
		throw new ValidationException("Turnstile response was empty");
	}
}