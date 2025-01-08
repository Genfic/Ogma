var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("postgres-password", secret: true);
var sqlUsername = builder.AddParameter("postgres-username", secret: true);

Console.WriteLine($"{sqlUsername.Resource.Value} / {sqlPassword.Resource.Value}");

var database = builder
	.AddPostgres("postgres", sqlUsername, sqlPassword)
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("ogma3-db");

builder
	.AddProject<Projects.Ogma3>("ogma3")
	.WithExternalHttpEndpoints()
	.WithReference(database)
	.WaitFor(database);

builder.Build().Run();