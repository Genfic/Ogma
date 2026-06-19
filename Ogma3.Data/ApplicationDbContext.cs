using AutoDbSetGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Constants;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Data;

[AutoDbContext]
public sealed partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext
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
}