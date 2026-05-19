using Ogma3.Data;
using Ogma3.MigrationService;
using Ogma3.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();

await host.RunAsync();
