using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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

public class DbSeedInitializer : IAsyncInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly OgmaUserManager _userManager;

    private JsonData Data { get; }

    public DbSeedInitializer(ApplicationDbContext context, OgmaUserManager userManager)
    {
        _context = context;
        _userManager = userManager;

        using var sr = new StreamReader("seed.json");
        var data = JsonSerializer.Deserialize<JsonData>(sr.ReadToEnd());

        if (data is not null)
        {
            Data = data;
        }
        else
        {
            Log.Fatal("Could not read seed.json file to seed the database");
        }
    }

    private sealed record JsonData(string[] Icons, Rating[] Ratings, string QuotesUrl);


    public async Task InitializeAsync()
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

        await _context.Roles.UpsertRange(roles)
            .On(r => r.NormalizedName)
            .WhenMatched((o, n) => new OgmaRole
            {
                Name = n.Name,
                IsStaff = n.IsStaff,
                Color = n.Color,
                Order = n.Order
            })
            .RunAsync();
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
        await _context.Ratings.UpsertRange(Data.Ratings)
            .On(i => i.Name)
            .NoUpdate()
            .RunAsync();
    }

    private async Task SeedIcons()
    {
        await _context.Icons.UpsertRange(Data.Icons.Select(s => new Icon { Name = s }))
            .On(i => i.Name)
            .NoUpdate()
            .RunAsync();
    }

    private async Task SeedQuotes()
    {
        if (await _context.Quotes.AnyAsync()) return;

        using var hc = new HttpClient();
        var json = await hc.GetStringAsync(Data.QuotesUrl);

        if (string.IsNullOrEmpty(json)) return;

        var quotes = JsonSerializer
            .Deserialize<ICollection<JsonQuote>>(json)
            ?.Select(q => new Quote { Body = q.Quote, Author = q.Author });

        if (quotes is null) return;

        _context.Quotes.AddRange(quotes);

        await _context.SaveChangesAsync();
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed record JsonQuote(string Quote, string Author);
}