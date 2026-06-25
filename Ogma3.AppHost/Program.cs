using Microsoft.Extensions.Configuration;
using Ogma3.AppHost.Helpers;

var builder = DistributedApplication.CreateBuilder(args);

var emulateProd = builder.Configuration.GetValue<bool>("emulate-prod");
var allowDirty = builder.Configuration.GetValue<bool>("allow-dirty");
var isProd = emulateProd || builder.ExecutionContext.IsPublishMode;

var shouldSeed = builder.AddParameter("should-seed", "false", publishValueAsDefault: true);
var postgresPassword = builder.AddParameter("postgres-password", secret: true);
var garnetPassword = builder.AddParameter("garnet-password", secret: true);

var infisical = new Dictionary<string, IResourceBuilder<ParameterResource>>
{
	["Infisical__ProjectId"] = builder.AddParameter("infisical-project-id", secret: true),
	["Infisical__ClientSecret"] = builder.AddParameter("infisical-client-secret", secret: true),
	["Infisical__MachineId"] = builder.AddParameter("infisical-machine-id", secret: true),
	["Infisical__Env"] = builder.AddParameter("infisical-env", isProd ? "prod" : "dev", publishValueAsDefault: true),
};

var git = GitHelpers.GetState();

if (git.IsDirty && !allowDirty)
{
	Console.Error.WriteLine("Repository has uncommited changes. Aborting.");
	Environment.Exit(1);
}

builder
	.AddDockerComposeEnvironment("ogma3-docker")
	.ConfigureComposeFile(file => {
		if (!file.Networks.TryGetValue("aspire", out var network)) return;

		network.DriverOpts["com.docker.network.enable_ipv6"] = "true";

		network.Ipam ??= new();
		network.Ipam.Config.Add(new() { ["subnet"] = "fd01::/80" });
	})
	.WithDashboard(db => {
		db.WithHostPort(8085);
		db.WithEnvironment("Dashboard__Frontend__BrowserToken", Password.Generate(64, true));
	})
	.WithSshDeploySupport();

var garnet = builder
	.AddGarnet("garnet", port: 6379, password: garnetPassword)
	.WithDataVolume()
	.WithPersistence();

var database = builder
	.AddPostgres("postgres", port: 5432, password: postgresPassword)
	.WithImageTag("18")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithEndpoint("tcp", e =>
	{
		e.Port = 5433;
		e.TargetPort = 5432;
		e.IsProxied = false;
	})
	.IfNot(emulateProd, b => b.WithPgWeb())
	.AddDatabase("ogma3-db");

var migrator = builder
	.AddProject<Projects.Ogma3_Migrator>("migrator")
	.WithReference(database)
	.WaitFor(database);

var genfic = builder
	.AddProject<Projects.Ogma3>("ogma3")
	.WithHttpEndpoint(port: 32773, targetPort: 8080)
	.WithExternalHttpEndpoints()
	.WithEnvironment("ASPNETCORE_ENVIRONMENT", isProd ? "Production" : "Development")
	.WithEnvironment("SHOULD_SEED", shouldSeed)
	.WithEnvironment("OTEL_DOTNET_EXPERIMENTAL_EFCORE_ENABLE_TRACE_DB_QUERY_PARAMETERS", "true")
	.WithEnvironment("OTEL_DOTNET_AUTO_ENTITYFRAMEWORKCORE_SET_DBSTATEMENT_FOR_TEXT", "true")
	// .WithEnvironment("DOTNET_SYSTEM_NET_DISABLEIPV4", "true")
	.WithEnvironment("Git__CommitHash", git.Hash)
	.WithEnvironment("Git__Branch", git.Branch)
	.WithEnvironment("Git__Dirty", git.IsDirty.ToString())
	.WithEnvironment("BUILD_TIME", DateTimeOffset.UtcNow.ToString("O"))
	.WithEnvironmentVariables(infisical)
	.WaitForCompletion(migrator)
	.WithReference(database)
	.WaitFor(database)
	.WithReference(garnet)
	.WaitFor(garnet);

if (!builder.ExecutionContext.IsPublishMode)
{
	builder
		.AddContainer("tunnel", "cloudflare/cloudflared")
		.WithEnvironment("TUNNEL_TOKEN", builder.Configuration["CLOUDFLARE_TUNNEL_TOKEN"])
		.WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway")
		.WithArgs("tunnel", "--no-autoupdate", "run", "--no-tls-verify")
		.ExcludeFromManifest()
		.IfNot(emulateProd, b => b.WithExplicitStart())
		.WaitFor(genfic);
}

builder.Build().Run();