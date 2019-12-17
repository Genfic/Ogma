using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Services.Attributes;
using Utils;

namespace Ogma3.Pages.MyStories
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IB2Client _b2Client;
        private readonly IConfiguration _config;

        public CreateModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IB2Client b2Client,
            IConfiguration config
        )
        {
            _context = context;
            _userManager = userManager;
            _b2Client = b2Client;
            _config = config;
        }

        public List<Rating> Ratings { get; set; }
        public SelectList TagOptions { get; set; }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(
                CTConfig.Story.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Story.MinTitleLength
            )]
            public string Title { get; set; }

            [Required]
            [StringLength(
                CTConfig.Story.MaxDescriptionLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Story.MinDescriptionLength
            )]
            public string Description { get; set; }

            [Required]
            [StringLength(
                CTConfig.Story.MaxHookLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Story.MinHookLength
            )]
            public string Hook { get; set; }

            [DataType(DataType.Upload)]
            [MaxFileSize(CTConfig.Story.CoverMaxWeight)]
            [AllowedExtensions(new[] {".jpg", ".jpeg", ".png"})]
            public IFormFile Cover { get; set; }

            [Required] 
            public int Rating { get; set; }

            [Required] 
            public List<int> Tags { get; set; }
        }

        public async Task OnGetAsync()
        {
            Ratings = await _context.Ratings.ToListAsync();
            TagOptions = new SelectList(await _context.Tags.ToListAsync(), nameof(Tag.Id), nameof(Tag.Name));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ratings = await _context.Ratings.ToListAsync();
            
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var rating = await _context.Ratings.FindAsync(Input.Rating);
                var tags = _context.Tags.Where(t => Input.Tags.Contains(t.Id));

                // Add story
                var story = new Story
                {
                    Author = user,
                    Title = Input.Title,
                    Slug = Input.Title.Friendlify(),
                    Description = Input.Description,
                    Hook = Input.Hook,
                    Rating = rating,
                };

                _context.Stories.Add(story);
                await _context.SaveChangesAsync();

                // Add tags and associations
                foreach (var tag in tags)
                {
                    await _context.StoryTags.AddAsync(new StoryTag
                    {
                        StoryId = story.Id,
                        TagId = tag.Id
                    });
                }
                
                // Handle cover upload
                if (Input.Cover != null && Input.Cover.Length > 0)
                {
                    var ext = Input.Cover.FileName.Split('.').Last();
                    var fileName = $"covers/{story.Id}-{story.Slug}.{ext}";
                
                    // Upload new one
                    await using var ms = new MemoryStream();
                    Input.Cover.CopyTo(ms);

                    var keepUploading = true;
                    var counter = 10;
                    while (keepUploading && counter >= 0)
                    {
                        try
                        {
                            var file = await _b2Client.Files.Upload(ms.ToArray(), fileName);
                            Console.WriteLine(file.FileName);
                            story.CoverId = file.FileId;
                            story.Cover = _config["cdn"] + fileName;
                            keepUploading = false;
                        }
                        catch (B2Exception e)
                        {
                            Console.WriteLine("⚠ Backblaze Error: " + e.Message);
                            counter--;
                        }
                    }
                }
                
                // Final save
                await _context.SaveChangesAsync();
                
                return RedirectToPage("./Index");
            }
            else
            {
                Console.WriteLine("===========================================");
                foreach (var error in ModelState.Values.SelectMany(msv => msv.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                Console.WriteLine("===========================================");
                return RedirectToPage();
            }
        }
    }
}
