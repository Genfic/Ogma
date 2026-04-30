using InfisicalConfiguration;
using Serilog;

namespace Ogma3.Infrastructure.Config.RemoteSecrets;

public static class RemoteSecretsExtensions
{
	extension(IHostApplicationBuilder builder)
	{
		public IConfigurationBuilder AddInfisical()
		{
			var opts = builder.Configuration.GetSection("Infisical").Get<InfisicalOptions>();
			if (opts is null)
			{
				Log.Error("Infisical configuration not found");
				return builder.Configuration;
			}

			var auth = new InfisicalAuthBuilder()
				.SetUniversalAuth(opts.MachineId, opts.ClientSecret)
				.Build();

			var config = new InfisicalConfigBuilder()
				.SetProjectId(opts.ProjectId)
				.SetEnvironment(opts.Env ?? "dev")
				.SetInfisicalUrl("https://eu.infisical.com")
				.SetAuth(auth)
				.Build();

			return builder.Configuration.AddInfisical(config);
		}

		public IHostApplicationBuilder BindRemoteConfigOptions()
		{
			builder.Services
				.AddOptions<Workers>()
				.Bind(builder.Configuration.GetSection("Workers"))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return builder;
		}
	}

	private sealed record InfisicalOptions
	(
		string ProjectId,
		string MachineId,
		string? Env,
		string ClientSecret
	);
}