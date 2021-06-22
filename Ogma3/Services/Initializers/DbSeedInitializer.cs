using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Ogma3.Services.Initializers
{
    public class DbSeedInitializer : IAsyncInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;
        private readonly RoleManager<OgmaRole> _roleManager;

        private JsonData Data { get; set; }
        
        public DbSeedInitializer(ApplicationDbContext context, OgmaUserManager userManager, RoleManager<OgmaRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            
            using var sr = new StreamReader("seed.json");
            var data = JsonSerializer.Deserialize<JsonData>(sr.ReadToEnd());
            if (data != null)
            {
                Data = data;
            }
            else
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> COULD NOT READ `seed.json`");
            }
        }
        
        
        public async Task InitializeAsync()
        {
            await SeedRoles();
            await SeedUserRoles();
            SeedRatings();
            SeedIcons();
            await SeedQuotes();
        }
        
        
        
        private async Task SeedRoles()
        {
            RoleBuilder rb;
            
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
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }

        private void SeedRatings()
        {
            foreach (var rating in Data.Ratings)
            {
                if (_context.Ratings.FirstOrDefault(r => r.Name == rating.Name) is null)
                {
                    _context.Ratings.Add(rating);
                }
                _context.SaveChanges();
            }
        }

        private void SeedIcons()
        {
            foreach (var i in Data.Icons)
            {
                if (_context.Icons.FirstOrDefault(ico => ico.Name == i) is null)
                {
                    _context.Icons.Add(new Icon {Name = i});
                }
            }

            _context.SaveChanges();
        }
        
        private async Task SeedQuotes()
        {
            if (await _context.Quotes.AnyAsync()) return;
            
            using var wc = new WebClient();
            var json = wc.DownloadString(Data.QuotesUrl);
            
            if (string.IsNullOrEmpty(json)) return;
            
            var quotes = JsonSerializer
                .Deserialize<ICollection<JsonQuote>>(json)
                ?.Select(q => new Quote{ Body = q.Quote, Author = q.Author});

            if (quotes == null) return;
            
            await _context.Quotes.AddRangeAsync(quotes);
            
            await _context.SaveChangesAsync();
        }
        
        #region deserialization classes
        
        private class JsonQuote
        {
            public string Quote { get; set; }
            public string Author { get; set; }
        }
        private class JsonData
        {
            public string[] Icons { get; set; }
            public Rating[] Ratings { get; set; }
            public string QuotesUrl { get; set; }
        }
        
        #endregion
    }
}