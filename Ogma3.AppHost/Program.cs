using Microsoft.Extensions.Configuration;
using Ogma3.AppHost.Helpers;

var builder = DistributedApplication.CreateBuilder(args);

var emulateProd = builder.Configuration.GetValue<bool>("emulate-prod");

var shouldSeed = builder.AddParameter("should-seed");

builder.AddDockerComposeEnvironment("ogma3-docker");

var garnet = builder
	.AddGarnet("garnet", port: 6379)
	.WithDataVolume()
	.WithPersistence();

var database = builder
	.AddPostgres("postgres", port: 5432)
	.WithImageTag("18")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithEndpoint("tcp", e =>
	{
		e.Port = 5433;
		e.TargetPort = 5432;
		e.IsProxied = false;
	})
	.If(!emulateProd, b => b.WithPgWeb())
	.AddDatabase("ogma3-db");

var genfic = builder
	.AddProject<Projects.Ogma3>("ogma3")
	.WithExternalHttpEndpoints()
	.WithEnvironment("ASPNETCORE_ENVIRONMENT", emulateProd ? "Production" : "Development")
	.WithEnvironment("SHOULD_SEED", shouldSeed)
	.WithEnvironment("OTEL_DOTNET_EXPERIMENTAL_EFCORE_ENABLE_TRACE_DB_QUERY_PARAMETERS", "true")
	.WithEnvironment("OTEL_DOTNET_AUTO_ENTITYFRAMEWORKCORE_SET_DBSTATEMENT_FOR_TEXT", "true")
	.WithReference(database)
	.WaitFor(database)
	.WithReference(garnet)
	.WaitFor(garnet);

builder
	.AddContainer("tunnel", "cloudflare/cloudflared")
	.WithEnvironment("TUNNEL_TOKEN", builder.Configuration["CLOUDFLARE_TUNNEL_TOKEN"])
	.WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway")
	.WithArgs("tunnel", "--no-autoupdate", "run")
	.ExcludeFromManifest()
	.If(!emulateProd, b => b.WithExplicitStart())
	.WaitFor(genfic);

builder.Build().Run();