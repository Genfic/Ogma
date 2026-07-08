using CompiledModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgSqlGenerators;

namespace Ogma3.Data;

public static class DbContextRegistration
{
	public static IHostApplicationBuilder AddApplicationDbContext(this IHostApplicationBuilder builder)
	{
		var dev = builder.Environment.IsDevelopment();

		builder.Services.AddDbContextPool<ApplicationDbContext>(options => {
			var conn = builder.Configuration.GetConnectionString("ogma3-db");

			if (conn is null)
			{
				throw new InvalidOperationException("Connection string 'ogma3-db' is not defined.");
			}

			if (dev)
			{
				if (conn.Trim() is not [.., ';'])
				{
					conn += ';';
				}
				conn += "Include Error Detail=True;";
			}

			options
				.UseNpgsql(conn, o => o
					.MapPostgresEnums()
					.SetPostgresVersion(18, 0)
				)
				.UseModel(ApplicationDbContextModel.Instance)
				// NOTE: uh-oh stinky poo-poo
				// But without it, for some godforsaken reason, the Migrator project wholeheartedly believes, that the
				// model has pending changes even if creating a new migration literally generates empty Up and Down methods.
				.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

			if (builder.Environment.IsDevelopment())
			{
				options
					.EnableSensitiveDataLogging()
					.EnableDetailedErrors();
			}
		});

		return builder;
	}
}