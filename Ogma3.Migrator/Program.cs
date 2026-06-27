using Ogma3.Data;
using Ogma3.Migrator;
using Ogma3.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<MigrationWorker>();
builder.Services
	.AddOpenTelemetry()
	.WithTracing(t => t.AddSource(MigrationWorker.ActivitySourceName));
builder.AddApplicationDbContext();

var host = builder.Build();
host.Run();