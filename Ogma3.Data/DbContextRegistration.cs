using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NpgSqlGenerators;
using Utils.Extensions;

namespace Ogma3.Data;

public static class DbContextRegistration
{
	public static IHostApplicationBuilder AddApplicationDbContext(this IHostApplicationBuilder builder)
	{
		var dev = builder.Environment.IsDevelopment();

		builder.Services.AddOptions<IdentityOptions>()
			.Configure(o => o.Stores.SchemaVersion = IdentitySchemaVersions.Version3);

		builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
		{
			var conn = builder.Configuration.GetConnectionString("ogma3-db")
				?? throw new InvalidOperationException("Connection string 'ogma3-db' is not defined.");

			if (dev)
			{
				if (conn.Trim() is not [.., ';'])
				{
					conn += ';';
				}
				conn += "Include Error Detail=True;";

				options
					.EnableSensitiveDataLogging()
					.EnableDetailedErrors();
			}

			options
				.UseNpgsql(conn, o => o
					.MapPostgresEnums()
					.SetPostgresVersion(18, 0)
				)
				.ConfigureWarnings(w => w
					.Ignore(CoreEventId.MappedPropertyIgnoredWarning)
					.If(dev, wr => wr.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning))
				);
		});

		return builder;
	}
}
