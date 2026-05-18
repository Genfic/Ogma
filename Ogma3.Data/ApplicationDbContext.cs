using AutoDbSetGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgSqlGenerators;
using Ogma3.Data.Constants;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Data;

[AutoDbContext]
public sealed partial class ApplicationDbContext
(
	DbContextOptions options,
	IConfiguration configuration,
	ILoggerFactory loggerFactory,
	IHostEnvironment environment
) : IdentityDbContext
<
	OgmaUser,
	OgmaRole,
	long,
	IdentityUserClaim<long>,
	UserRole,
	IdentityUserLogin<long>,
	IdentityRoleClaim<long>,
	IdentityUserToken<long>
>(options)
{
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Collations
		builder
			.HasCollation(
				name: PgConstants.CollationNames.CaseInsensitive,
				locale: "und-u-ks-level2",
				provider: "icu",
				deterministic: false
			)
			.HasCollation(
				name: PgConstants.CollationNames.CaseInsensitiveNoAccent,
				locale: "und-u-ks-level1",
				provider: "icu",
				deterministic: false
			);

		// Extensions
		builder
			.HasPostgresExtension("uuid-ossp")
			.HasPostgresExtension("tsm_system_rows")
			.HasPostgresExtension("citext")
			.HasPostgresExtension("intarray");

		// Load model configurations
		builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		if (optionsBuilder.IsConfigured) return;

		var conn = configuration.GetConnectionString("ogma3-db");
		optionsBuilder
			.UseLoggerFactory(loggerFactory)
			.UseNpgsql(conn, o => o
				.MapPostgresEnums()
				.SetPostgresVersion(18, 0)
			);

		if (environment.IsDevelopment())
		{
			optionsBuilder
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors();
		}
	}
}