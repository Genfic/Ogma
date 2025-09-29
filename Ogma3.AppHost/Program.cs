using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var emulateProd = args.Contains("--emulate-prod");

var shouldSeed = builder.AddParameter("seed");

builder.AddDockerComposeEnvironment("ogma3-docker");

var garnet = builder.AddGarnet("garnet")
	.WithDataVolume()
	.WithPersistence();

var database = builder
	.AddPostgres("postgres")
	.WithImageTag("17.6")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("ogma3-db");

builder
	.AddProject<Ogma3>("ogma3", launchProfileName: emulateProd ? "Ogma3 Prod" : "Ogma3")
	.WithExternalHttpEndpoints()
	.WithEnvironment("SHOULD_SEED", shouldSeed)
	.WithReference(database)
	.WaitFor(database)
	.WithReference(garnet)
	.WaitFor(garnet);

builder.Build().Run();