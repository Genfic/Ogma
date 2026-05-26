using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.Config.RemoteSecrets;

public static partial class RemoteSecretsExtensions
{
	extension(IHostApplicationBuilder builder)
	{
		public async Task AddInfisicalAsync(int maxRetries = 6)
		{
			var opts = builder.Configuration.GetSection("Infisical").Get<InfisicalOptions>();
			if (opts is null)
			{
				throw new InvalidOperationException("Infisical configuration is missing");
			}

			ValidationException.ThrowIfInvalid(opts);

			var delay = TimeSpan.FromSeconds(2);

			for (var attempt = 1; attempt <= maxRetries; attempt++)
			{
				var step = "none";
				try
				{
					using var client = new HttpClient();
					client.BaseAddress = new Uri("https://eu.infisical.com");

					step = "start";

					var token = await Authorize(client, opts);
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					step = "authorized";

					var secrets = await Load(client, opts);
					builder.Configuration.AddInMemoryCollection(secrets);

					step = "loaded";

					return;
				}
				catch (Exception ex) when (attempt < maxRetries && ex is HttpRequestException or SocketException)
				{
					await Console.Error.WriteLineAsync($"[Infisical] Network not ready (Attempt {attempt}/{maxRetries}), step {step}, retrying in {delay.TotalSeconds}s... Error {ex.Message}");
					await Task.Delay(delay);
					delay *= 2;
				}
			}
		}
	}

	private static async Task<string> Authorize(HttpClient client, InfisicalOptions options)
	{
		var body = new InfisicalAuthRequest(options.MachineId, options.ClientSecret);
		const string url = "/api/v1/auth/universal-auth/login";

		var response = await client.PostAsJsonAsync(url, body, InfisicalJsonContext.Default.InfisicalAuthRequest);

		response.EnsureSuccessStatusCode();

		var data = await response.Content.ReadFromJsonAsync(InfisicalJsonContext.Default.MachineIdentityLogin);

		return data?.AccessToken ?? throw new NullReferenceException("Infisical response was null");
	}

	private static async Task<Dictionary<string, string?>> Load(HttpClient client, InfisicalOptions options)
	{
		var url = $"/api/v3/secrets/raw?environment={options.Env}&workspaceId={options.ProjectId}&secretPath={options.SecretPath}&include_imports=true&expandSecretReferences={options.ExpandSecretReferences.ToString().ToLower()}";

		var response = await client.GetFromJsonAsync(url, InfisicalJsonContext.Default.SecretsList);
		if (response is null)
		{
			throw new NullReferenceException("Infisical response was null");
		}

		response.Secrets.Reverse();

		var prefix = options.Prefix switch
		{
			null or "" => "",
			[.., ':'] => options.Prefix,
			_ => options.Prefix + ':',
		};

		var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
		foreach (var secret in response.Secrets)
		{
			var key = $"{prefix}{secret.SecretKey.Replace("__", ":")}";
			dict[key] = secret.SecretValue;
		}
		return dict;
	}

	[Validate]
	private sealed partial record InfisicalOptions
	(
		[property: NotEmpty] string ProjectId,
		[property: NotEmpty] string MachineId,
		[property: NotEmpty] string ClientSecret,
		string Env = "dev",
		string? Prefix = null,
		string SecretPath = "/",
		bool ExpandSecretReferences = true
	) : IValidationTarget<InfisicalOptions>;
}

internal sealed record MachineIdentityLogin(string AccessToken);
internal sealed record InfisicalAuthRequest(string ClientId, string ClientSecret);
internal sealed record Secret(string SecretKey, string? SecretValue);
internal sealed record SecretsList(List<Secret> Secrets);


[JsonSerializable(typeof(InfisicalAuthRequest))]
[JsonSerializable(typeof(MachineIdentityLogin))]
[JsonSerializable(typeof(SecretsList))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class InfisicalJsonContext : JsonSerializerContext;