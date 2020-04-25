using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Services.Initializers
{
    public class DbSeedInitializer : IAsyncInitializer
    {

        public ApplicationDbContext Ctx { get; set; }
        public OgmaUserManager UserManager { get; set; }
        public RoleManager<Role> RoleManager { get; set; }
        
        public DbSeedInitializer(ApplicationDbContext ctx, OgmaUserManager userManager, RoleManager<Role> roleManager)
        {
            Ctx = ctx;
            UserManager = userManager;
            RoleManager = roleManager;
        }
        
        
        public async Task InitializeAsync()
        {
            await SeedRoles(RoleManager);
            await SeedUserRoles(UserManager);
            SeedRatings(Ctx);
            SeedIcons(Ctx);
        }
        
        
        
        private static async Task SeedRoles (RoleManager<Role> roleManager)
        {
            if (await roleManager.RoleExistsAsync("Admin")) return;
            
            var role = new Role { Name = "Admin" };
            await roleManager.CreateAsync(role);
        }

        private static async Task SeedUserRoles(OgmaUserManager userManager)
        {
            var user = await userManager.FindByNameAsync("Angius");
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        private static void SeedRatings(ApplicationDbContext ctx)
        {
            Rating[] ratings =
            {
                new Rating {Name = "Everyone", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"},
                new Rating {Name = "Teen", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"},
                new Rating {Name = "Mature", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"},
                new Rating {Name = "Adult", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"}
            };

            foreach (var rating in ratings)
            {
                if (ctx.Ratings.FirstOrDefault(r => r.Name == rating.Name) == null)
                {
                    ctx.Ratings.Add(rating);
                }
                ctx.SaveChanges();
            }
        }

        private static void SeedIcons(ApplicationDbContext ctx)
        {
            string[] icons =
            {
                "book",
                "bookmark_border",
                "check_circle",
                "delete",
                "eco",
                "explore",
                "extension",
                "face",
                "favorite_border",
                "fingerprint",
                "star_border",
                "report_problem",
                "thumb_up",
                "thumb_down",
                "visibility",
                "new_releases",
                "outlined_flag",
                "toys",
                "palette",
                "casino",
                "spa"
            };
            foreach (var i in icons)
            {
                if (ctx.Icons.FirstOrDefault(ico => ico.Name == i) == null)
                {
                    ctx.Icons.Add(new Icon {Name = i});
                }
            }

            ctx.SaveChanges();
        }
    }
}