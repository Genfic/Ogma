using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Services;
using Ogma3.Services.Attributes;
using Utils.Extensions;

namespace Ogma3.Pages.Stories
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly FileUploader _uploader;

        public CreateModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            FileUploader uploader)
        {
            _context = context;
            _userManager = userManager;
            _uploader = uploader;
        }

        public List<Rating> Ratings { get; set; }

        [BindProperty] 
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(
                CTConfig.CStory.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.CStory.MinTitleLength
            )]
            public string Title { get; set; }

            [Required]
            [StringLength(
                CTConfig.CStory.MaxDescriptionLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.CStory.MinDescriptionLength
            )]
            public string Description { get; set; }

            [Required]
            [StringLength(
                CTConfig.CStory.MaxHookLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.CStory.MinHookLength
            )]
            public string Hook { get; set; }

            [DataType(DataType.Upload)]
            [MaxFileSize(CTConfig.CStory.CoverMaxWeight)]
            [AllowedExtensions(new[] {".jpg", ".jpeg", ".png"})]
            public IFormFile Cover { get; set; }

            [Required] 
            public long Rating { get; set; }

            [Required] 
            public List<long> Tags { get; set; }
        }

        public async Task OnGetAsync()
        {
            Input = new InputModel();
            Ratings = await _context.Ratings
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ratings = await _context.Ratings.ToListAsync();
            
            Console.WriteLine("— " + (ModelState.IsValid ? "State Valid" : "State Invalid"));

            foreach (var (key, value) in ModelState)
            {
                Console.WriteLine($"|— {key}");
                foreach (var error in value.Errors)
                {
                    Console.WriteLine($"| |— {error.ErrorMessage}");
                    Console.WriteLine($"| |  \\— {error.Exception?.Message}");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

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
                Tags = tags
            };

            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();

            // Upload cover
            if (Input.Cover != null && Input.Cover.Length > 0)
            {
                var file = await _uploader.Upload(Input.Cover, "covers", $"{story.Id}-{story.Slug}");
                story.CoverId = file.FileId;
                story.Cover = file.Path;
                // Final save
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Story", new {id = story.Id, slug = story.Slug});
        }
    }
}
