using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.AspNetCore.Identity;
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
    private readonly RoleManager<OgmaRole> _roleManager;

    private JsonData Data { get; }
        
    public DbSeedInitializer(ApplicationDbContext context, OgmaUserManager userManager, RoleManager<OgmaRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
            
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
        var adminRole = new OgmaRole { Name = RoleNames.Admin, IsStaff = true, Color = "#ffaa00", Order = byte.MaxValue};
        await new RoleBuilder(adminRole, _roleManager).Build();
            
        var modRole = new OgmaRole { Name = RoleNames.Moderator, IsStaff = true, Color = "#aaff00", Order = byte.MaxValue - 5};
        await new RoleBuilder(modRole, _roleManager).Build();
            
        var helperRole = new OgmaRole { Name = RoleNames.Helper, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 10};
        await new RoleBuilder(helperRole, _roleManager).Build();

        var reviewerRole = new OgmaRole { Name = RoleNames.Reviewer, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 15};
        await new RoleBuilder(reviewerRole, _roleManager).Build();
            
        var supporterRole = new OgmaRole { Name = RoleNames.Supporter, IsStaff = false, Color = "#ffdd11", Order = byte.MaxValue - 20};
        await new RoleBuilder(supporterRole, _roleManager).Build();
    }

    private async Task SeedUserRoles()
    {
        var user = await _userManager.FindByNameAsync("Angius");
        if (user is not null)
        {
            await _userManager.AddToRoleAsync(user, RoleNames.Admin);
        }
    }

    private async Task SeedRatings()
    {
        foreach (var rating in Data.Ratings)
        {
            if (!await _context.Ratings.AnyAsync(r => r.Name == rating.Name))
            {
                _context.Ratings.Add(rating);
            }
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedIcons()
    {
        foreach (var i in Data.Icons)
        {
            if (!await _context.Icons.AnyAsync(ico => ico.Name == i))
            {
                _context.Icons.Add(new Icon { Name = i });
            }
        }

        await _context.SaveChangesAsync();
    }
        
    private async Task SeedQuotes()
    {
        if (await _context.Quotes.AnyAsync()) return;
            
        using var hc = new HttpClient();
        var json = await hc.GetStringAsync(Data.QuotesUrl);
            
        if (string.IsNullOrEmpty(json)) return;
            
        var quotes = JsonSerializer
            .Deserialize<ICollection<JsonQuote>>(json)
            ?.Select(q => new Quote{ Body = q.Quote, Author = q.Author});

        if (quotes is null) return;
            
        _context.Quotes.AddRange(quotes);
            
        await _context.SaveChangesAsync();
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed record JsonQuote(string Quote, string Author);
        
}