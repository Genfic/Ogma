using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Extensions.Hosting.AsyncInitialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Data.Images;
using Ogma3.Data.Quotes;
using Ogma3.Data.Ratings;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;
using Utils;

namespace Ogma3.Services.Initializers;

[UsedImplicitly]
public sealed class DbSeedInitializer : IAsyncInitializer
{
	private readonly ApplicationDbContext _context;
	private readonly OgmaUserManager _userManager;
	private readonly ILogger<DbSeedInitializer> _logger;
	private readonly IHttpClientFactory _clientFactory;

	private readonly JsonData _data;

	public DbSeedInitializer(ApplicationDbContext context, OgmaUserManager userManager, ILogger<DbSeedInitializer> logger, IHttpClientFactory clientFactory)
	{
		_context = context;
		_userManager = userManager;
		_logger = logger;
		_clientFactory = clientFactory;

		using var sr = new StreamReader("seed.json5");
		var data = JsonSerializer.Deserialize(sr.ReadToEnd(), JsonDataContext.Default.JsonData);

		if (data is not null)
		{
			_data = data;
		}
		else
		{
			_logger.LogCritical("Could not read seed.json5 file to seed the database");
			throw new NullReferenceException("Json data was null");
		}
	}

	public async Task InitializeAsync(CancellationToken ct)
	{
		_logger.LogInformation("Async initialization started.");

		var timer = new Stopwatch();
		timer.Start();

		await Time(SeedRoles, nameof(SeedRoles));
		await Time(SeedUsers, nameof(SeedUsers));
		await Time(SeedRatings, nameof(SeedRatings));
		await Time(SeedIcons, nameof(SeedIcons));
		await Time(SeedQuotes, nameof(SeedQuotes));

		timer.Stop();
		_logger.LogInformation("Async initialization took {Time} ms", timer.ElapsedMilliseconds);
	}

	private async Task SeedRoles()
	{
		var roles = new[]
		{
			new OgmaRole { Name = RoleNames.Admin, IsStaff = true, Color = "#ffaa00", Order = byte.MaxValue }.Normalize(),
			new OgmaRole { Name = RoleNames.Moderator, IsStaff = true, Color = "#aaff00", Order = byte.MaxValue - 5 }.Normalize(),
			new OgmaRole { Name = RoleNames.Helper, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 10 }.Normalize(),
			new OgmaRole { Name = RoleNames.Reviewer, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 15 }.Normalize(),
			new OgmaRole { Name = RoleNames.Supporter, IsStaff = false, Color = "#ffdd11", Order = byte.MaxValue - 20 }.Normalize(),
		};

		await BulkUpsert(_context.Roles, roles, r => r.NormalizedName);
	}

	private const string Email = "admin@genfic.net";
	public async Task SeedUsers()
	{
		var exists = await _userManager.FindByEmailAsync(Email);
		if (exists is not null) return;

		var adminRole = await _context.Roles.SingleOrDefaultAsync(r => r.NormalizedName == RoleNames.Admin);

		var alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()".AsSpan();
		var password = new string(Random.Shared.GetItems(alpha, 20));
		var user = new OgmaUser
		{
			UserName = "Angius",
			Email = Email,
			Avatar = new Image
			{
				Url = Gravatar.Generate(Email),
			},
			Roles = adminRole is null ? [] : [ adminRole ],
		};
		var result = await _userManager.CreateAsync(user, password);
		if (result.Succeeded)
		{
			_logger.LogCritical("Admin user created with password {Password}. Change it ASAP.", password);
		}
		else
		{
			_logger.LogCritical("Creating admin failed with errors: {Errors}", result.Errors.Select(e => e.Description));
		}
	}

	private async Task SeedRatings()
	{
		await BulkUpsert(_context.Ratings, _data.Ratings, r => r.Name);
	}

	private async Task SeedIcons()
	{
		var icons = _data.Icons.Select(s => new Icon { Name = s });

		await BulkUpsert(_context.Icons, icons, i => i.Name);
	}

	private async Task SeedQuotes()
	{
		if (await _context.Quotes.AnyAsync()) return;

		using var hc = _clientFactory.CreateClient();
		var json = await hc.GetFromJsonAsync(_data.QuotesUrl, JsonQuoteContext.Default.JsonQuoteArray);

		var quotes = json?.Select(q => new Quote { Body = q.Quote, Author = q.Author });
		if (quotes is null) return;

		_context.Quotes.AddRange(quotes);

		await _context.SaveChangesAsync();
	}

	private async Task BulkUpsert<TEntity, TKey>(DbSet<TEntity> source, IEnumerable<TEntity> entries, Expression<Func<TEntity, TKey>> extractor) where TEntity : class
	{
		var existing = await source
			.Select(extractor)
			.ToListAsync();

		var exceptor = extractor.Compile();
		var toAdd = entries.ExceptBy(existing, exceptor);

		source.AddRange(toAdd);

		await _context.SaveChangesAsync();
	}

	private async Task Time(Func<Task> func, string name)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		await func();

		stopwatch.Stop();
		_logger.LogInformation("{Name} executed in {Time}ms", name, stopwatch.ElapsedMilliseconds);
	}
}

public sealed record JsonData(string[] Icons, Rating[] Ratings, string QuotesUrl);

[JsonSerializable(typeof(JsonData))]
[JsonSourceGenerationOptions(AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip)]
public sealed partial class JsonDataContext : JsonSerializerContext;

public sealed record JsonQuote(string Quote, string Author);

[JsonSerializable(typeof(JsonQuote[]))]
public sealed partial class JsonQuoteContext : JsonSerializerContext;