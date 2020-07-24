using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Services;
using Ogma3.Services.Attributes;
using Utils.Extensions;

namespace Ogma3.Pages.Stories
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IB2Client _b2Client;
        private readonly IConfiguration _config;
        private readonly OgmaUserManager _userManager;
        private readonly FileUploader _uploader;

        public Story Story { get; set; }

        public EditModel(
            ApplicationDbContext context,
            IB2Client b2Client,
            IConfiguration config, OgmaUserManager userManager, FileUploader uploader)
        {
            _context = context;
            _b2Client = b2Client;
            _config = config;
            _userManager = userManager;
            _uploader = uploader;
        }

        public List<Rating> Ratings { get; set; }
        
        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            public long Id { get; set; }
            
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
            public EStoryStatus Status { get; set; }

            [Required] 
            public List<long> Tags { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Get story to edit and make sure author matches logged in user
            Story = await _context.Stories
                .Include(s => s.StoryTags)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Story == null) return NotFound();
            if (!Story.Author.IsLoggedIn(User)) return RedirectToPage("Index");
            
            // Fill InputModel
            Input = new InputModel
            {
                Id = Story.Id,
                Title = Story.Title,
                Description = Story.Description,
                Hook = Story.Hook,
                Rating = Story.Rating.Id,
                Tags = Story.StoryTags.Select(st => st.TagId).ToList(),
                Status = Story.Status
            };
            
            // Fill Ratings dropdown
            Ratings = await _context.Ratings.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Ratings = await _context.Ratings.ToListAsync();
            
            if (ModelState.IsValid)
            {
                var tags = await _context.Tags.Where(t => Input.Tags.Contains(t.Id)).ToListAsync();

                // Get logged in user
                var user = await _userManager.GetUserAsync(User);
                // Get the story and make sure the logged-in user matches author
                Story = await _context.Stories
                    .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                    .Include(s => s.Rating)
                    .Include(s => s.Author)
                    .FirstOrDefaultAsync(s => s.Id == id && s.Author == user);
                // 404 if none found
                if (Story == null) return NotFound();
                
                // Update story
                Story.Title = Input.Title;
                Story.Slug = Input.Title.Friendlify();
                Story.Description = Input.Description;
                Story.Hook = Input.Hook;
                Story.Rating = await _context.Ratings.FindAsync(Input.Rating);
                Story.Tags = tags;
                Story.Status = Input.Status;
                _context.Update(Story);
                await _context.SaveChangesAsync();
                
                // Handle cover upload
                if (Input.Cover != null && Input.Cover.Length > 0)
                {
                    // Upload cover
                    var file = await _uploader.Upload(Input.Cover, "covers", $"{Story.Id}-{Story.Slug}");
                    Story.CoverId = file.FileId;
                    Story.Cover = file.Path;
                    // Final save
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToPage("../Story", new { id = Story.Id, slug = Story.Slug });
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }
}
