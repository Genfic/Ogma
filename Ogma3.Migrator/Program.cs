using Ogma3.Migrator;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// IdentityDbContext reads the store schema version from IOptions<IdentityOptions> in the
// application service provider; without it the design-time model would fall back to
// schema version 1 and scaffolding would try to drop v3 tables (e.g. passkeys)
builder.Services.Configure<IdentityOptions>(o => o.Stores.SchemaVersion = IdentitySchemaVersions.Version3);

builder.Services.AddHostedService<MigrationWorker>();
builder.Services
	.AddOpenTelemetry()
	.WithTracing(t => t.AddSource(MigrationWorker.ActivitySourceName));
builder.AddApplicationDbContext();

var host = builder.Build();
host.Run();