using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var emulateProd = args.Contains("--emulate-prod");

var shouldSeed = builder.AddParameter("should-seed");

builder.AddDockerComposeEnvironment("ogma3-docker");

var garnet = builder
	.AddGarnet("garnet", port: 6379)
	.WithDataVolume()
	.WithPersistence();

var database = builder
	.AddPostgres("postgres", port: 5432)
	.WithImageTag("18.0")
	.WithEnvironment("PGDATA", "/var/lib/postgresql/data")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("ogma3-db");

var genfic = builder
	.AddProject<Ogma3>("ogma3", launchProfileName: emulateProd ? "Ogma3 Prod" : "Ogma3")
	.WithExternalHttpEndpoints()
	.WithEnvironment("SHOULD_SEED", shouldSeed)
	.WithReference(database)
	.WaitFor(database)
	.WithReference(garnet)
	.WaitFor(garnet);

builder.AddContainer("tunnel", "cloudflare/cloudflared")
	.WithEnvironment("TUNNEL_TOKEN", builder.Configuration["CLOUDFLARE_TUNNEL_TOKEN"])
	.WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway")
	.WithArgs("tunnel", "--no-autoupdate", "run")
	.WithReference(genfic)
	.WaitFor(genfic)
	.WithExplicitStart();

builder.Build().Run();