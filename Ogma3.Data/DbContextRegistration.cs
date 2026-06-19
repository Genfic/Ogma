using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgSqlGenerators;

namespace Ogma3.Data;

public static class DbContextRegistration
{
	public static IHostApplicationBuilder AddApplicationDbContext(this IHostApplicationBuilder builder)
	{
		builder.Services.AddDbContextPool<ApplicationDbContext>(options => {
			var conn = builder.Configuration.GetConnectionString("ogma3-db");
			options
				.UseNpgsql(conn, o => o
					.MapPostgresEnums()
					.SetPostgresVersion(18, 0)
				);

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