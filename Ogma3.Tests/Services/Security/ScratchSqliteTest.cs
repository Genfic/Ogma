namespace Ogma3.Tests.Services.Security;

// Scratch test — DELETE ME after checking SQLite compatibility
public sealed class ScratchSqliteTest
{
	[Test]
	public async Task PrunedIdentityModelBuildsOnSqlite()
	{
		var connection = new SqliteConnection("DataSource=:memory:");
		await connection.OpenAsync();

		var options = new DbContextOptionsBuilder<SecurityDbContext>()
			.UseSqlite(connection)
			.Options;

		await using var context = new SecurityDbContext(options);
		var created = await context.Database.EnsureCreatedAsync();
		await Assert.That(created).IsTrue();
	}
}

/// <summary>
/// Provider-neutral context for security-service tests: Identity's own tables plus the
/// SecurityTransactions table, with OgmaUser's non-Identity graph pruned out.
/// </summary>
public sealed class SecurityDbContext(DbContextOptions options)
	: IdentityDbContext<
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
	public DbSet<SecurityTransaction> SecurityTransactions => Set<SecurityTransaction>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfiguration(new SecurityTransactionConfiguration());

		// Prune OgmaUser's non-Identity graph: navigations (and their FKs), skip navigations
		// and complex properties (passkeys) are not needed for these tests, and parts of the
		// graph are PostgreSQL-specific
		var userEntity = builder.Entity<OgmaUser>();
		userEntity.Ignore(u => u.Avatar);
		userEntity.Ignore(u => u.CommentThread);
		userEntity.Ignore(u => u.Stories);
		userEntity.Ignore(u => u.Blogposts);
		userEntity.Ignore(u => u.BlacklistedRatings);
		userEntity.Ignore(u => u.BlacklistedTags);
		userEntity.Ignore(u => u.Blockers);
		userEntity.Ignore(u => u.Blocking);
		userEntity.Ignore(u => u.Followers);
		userEntity.Ignore(u => u.Following);
		userEntity.Ignore(u => u.SubscribedThreads);
		userEntity.Ignore(u => u.Reports);
		userEntity.Ignore(u => u.Infractions);
		userEntity.Ignore(u => u.Notifications);
		userEntity.Ignore(u => u.Subscription);

		var user = userEntity.Metadata;
		foreach (var fk in user.GetForeignKeys().ToList()) user.RemoveForeignKey(fk);
		foreach (var complex in user.GetComplexProperties().ToList()) user.RemoveComplexProperty(complex);

		// Ignore every non-Identity navigation everywhere so the relationships (and the entity
		// types they point to) fall out of the model through the cleanup convention
		var allowed = new[]
		{
			typeof(OgmaUser), typeof(OgmaRole), typeof(UserRole),
			typeof(IdentityUserClaim<long>), typeof(IdentityUserLogin<long>),
			typeof(IdentityRoleClaim<long>), typeof(IdentityUserToken<long>),
			typeof(SecurityTransaction),
		};

		for (var pass = 0; pass < 5; pass++)
		{
			var targets = builder.Model.GetEntityTypes()
				.Where(t => !allowed.Contains(t.ClrType) || t.ClrType == typeof(OgmaUser) || t.ClrType == typeof(OgmaRole))
				.ToList();

			var extras = targets.Where(t => !allowed.Contains(t.ClrType)).ToList();

			if (extras.Count == 0 && pass > 0) break;

			foreach (var entityType in targets)
			{
				var entityBuilder = builder.Entity(entityType.Name);

				foreach (var skip in entityType.GetSkipNavigations().ToList())
				{
					if (!allowed.Contains(skip.JoinEntityType?.ClrType)) entityBuilder.Ignore(skip.Name);
				}

				foreach (var nav in entityType.GetNavigations().ToList())
				{
					if (!allowed.Contains(nav.TargetEntityType.ClrType)) entityBuilder.Ignore(nav.Name);
				}
			}

			if (extras.Count == 0) break;

			// Entity types that are still present after ignoring are unreachable; remove
			// their FKs so the next pass (or the cleanup convention) can drop them
			foreach (var entityType in extras)
			{
				foreach (var fk in entityType.GetForeignKeys().ToList()) entityType.RemoveForeignKey(fk);
				foreach (var complex in entityType.GetComplexProperties().ToList()) entityType.RemoveComplexProperty(complex);
				try
				{
					builder.Model.RemoveEntityType(entityType.Name);
				}
				catch
				{
					// Dependent on other removed-later types; retried next pass
				}
			}
		}
	}
}
