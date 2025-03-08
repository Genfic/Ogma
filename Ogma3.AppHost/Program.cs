var builder = DistributedApplication.CreateBuilder(args);

var emulateProd = args.Contains("--emulate-prod");

var database = builder
	.AddPostgres("postgres")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("ogma3-db");

builder
	.AddProject<Projects.Ogma3>("ogma3", launchProfileName: emulateProd ? "Ogma3 Prod" : "Ogma3")
	.WithExternalHttpEndpoints()
	.WithReference(database)
	.WaitFor(database);

builder.Build().Run();