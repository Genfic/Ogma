using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ogma3;
using Ogma3.Data;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Ogma3.Tests.Integration;

/// <summary>
/// A <see cref="WebApplicationFactory{TProgram}"/> that spins up a real PostgreSQL container
/// and a Redis-compatible container for each test session, replacing the production
/// connection strings so tests run in complete isolation.
/// </summary>
public sealed class OgmaApiFactory : WebApplicationFactory<Program>, IAsyncInitializer, IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:18")
        .WithDatabase("ogma3_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:7")
        .Build();

    // IAsyncInitializer — called by TUnit before any test that depends on this fixture
    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _redis.StartAsync());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Replace the real DbContext registration with one pointing at the container
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    _postgres.GetConnectionString(),
                    npgsql => npgsql
                        .MapPostgresEnums()
                        .SetPostgresVersion(18, 0)
                )
            );

            // Replace distributed Redis cache with the Testcontainer instance
            services.RemoveAll<StackExchange.Redis.IConnectionMultiplexer>();
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = _redis.GetConnectionString();
            });

            // Ensure the schema is created fresh for every factory instance
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _redis.DisposeAsync();
        await base.DisposeAsync();
    }
}
