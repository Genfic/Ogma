using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var emulateProd = args.Contains("--emulate-prod");

builder.AddDockerComposeEnvironment("ogma3-docker");

var garnet = builder.AddGarnet("garnet")
	.WithDataVolume()
	.WithPersistence();

var database = builder
	.AddPostgres("postgres")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("ogma3-db");

builder
	.AddProject<Ogma3>("ogma3", launchProfileName: emulateProd ? "Ogma3 Prod" : "Ogma3")
	.WithExternalHttpEndpoints()
	.WithReference(database)
	.WaitFor(database)
	.WithReference(garnet)
	.WaitFor(garnet);

builder.Build().Run();