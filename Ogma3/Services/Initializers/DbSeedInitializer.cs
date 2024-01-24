using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Data.Quotes;
using Ogma3.Data.Ratings;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.Constants;
using Serilog;

namespace Ogma3.Services.Initializers;

public abstract class DbSeedInitializer : IAsyncInitializer
{
	private readonly ApplicationDbContext _context;
	private readonly OgmaUserManager _userManager;

	private readonly JsonData _data;

	protected DbSeedInitializer(ApplicationDbContext context, OgmaUserManager userManager)
	{
		_context = context;
		_userManager = userManager;

		using var sr = new StreamReader("seed.json");
		var data = JsonSerializer.Deserialize<JsonData>(sr.ReadToEnd());

		if (data is not null)
		{
			_data = data;
		}
		else
		{
			Log.Fatal("Could not read seed.json file to seed the database");
			throw new NullReferenceException("Json data was null");
		}
	}

	private sealed record JsonData(string[] Icons, Rating[] Ratings, string QuotesUrl);


	public async Task InitializeAsync(CancellationToken ct)
	{
		await SeedRoles();
		await SeedUserRoles();
		await SeedRatings();
		await SeedIcons();
		await SeedQuotes();
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

	private async Task SeedUserRoles()
	{
		var user = await _context.Users
			.Where(u => u.NormalizedUserName == "ANGIUS")
			.Where(u => u.Roles.All(r => r.NormalizedName != RoleNames.Admin.ToUpper()))
			.FirstOrDefaultAsync();
		if (user is not null)
		{
			await _userManager.AddToRoleAsync(user, RoleNames.Admin);
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

		using var hc = new HttpClient();
		var json = await hc.GetStringAsync(_data.QuotesUrl);

		if (string.IsNullOrEmpty(json)) return;

		var quotes = JsonSerializer
			.Deserialize<ICollection<JsonQuote>>(json)
			?.Select(q => new Quote { Body = q.Quote, Author = q.Author });

		if (quotes is null) return;

		_context.Quotes.AddRange(quotes);

		await _context.SaveChangesAsync();
	}

	private async Task BulkUpsert<TEntity, TKey>(DbSet<TEntity> source, IEnumerable<TEntity> entries, Func<TEntity, TKey> extractor) where TEntity : class
	{
		var existing = await source
			.Select(x => extractor(x))
			.ToListAsync();

		var toAdd = entries.ExceptBy(existing, extractor);
		
		source.AddRange(toAdd);

		await _context.SaveChangesAsync();
	}

	// ReSharper disable once ClassNeverInstantiated.Local
	private sealed record JsonQuote(string Quote, string Author);
}