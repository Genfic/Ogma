using AutoDbSetGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Serilog;

namespace Ogma3.Data;

[AutoDbContext]
public sealed partial class ApplicationDbContext : IdentityDbContext
<
	OgmaUser,
	OgmaRole,
	long,
	IdentityUserClaim<long>,
	UserRole,
	IdentityUserLogin<long>,
	IdentityRoleClaim<long>,
	IdentityUserToken<long>
>
{
	private readonly ILoggerFactory _myLoggerFactory;

	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
		_myLoggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Extensions
		builder
			.HasPostgresExtension("uuid-ossp")
			.HasPostgresExtension("tsm_system_rows");

		// Load model configurations
		builder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder
			.UseLoggerFactory(_myLoggerFactory);
	}
}