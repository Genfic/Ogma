using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Stories
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploader _uploader;
        private readonly OgmaConfig _config;

        public CreateModel(ApplicationDbContext context, FileUploader uploader, OgmaConfig config)
        {
            _context = context;
            _uploader = uploader;
            _config = config;
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

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            // Get logged in user
            var uid = User.GetNumericId();

            // Return if not logged in
            if (uid == null) return Unauthorized();
            
            var rating = await _context.Ratings.FindAsync(Input.Rating);
            var tags = _context.Tags.Where(t => Input.Tags.Contains(t.Id));

            // Add story
            var story = new Story
            {
                AuthorId = (long) uid,
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
                var file = await _uploader.Upload(
                    Input.Cover, 
                    "covers", 
                    $"{story.Id}-{story.Slug}",
                    _config.StoryCoverWidth,
                    _config.StoryCoverHeight
                );
                story.CoverId = file.FileId;
                story.Cover = file.Path;
                // Final save
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Story", new {id = story.Id, slug = story.Slug});
        }
    }
}
