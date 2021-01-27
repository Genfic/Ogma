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
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Stories
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploader _uploader;
        private readonly OgmaConfig _config;
        private readonly NotificationsRepository _notificationsRepo;

        public CreateModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig config, NotificationsRepository notificationsRepo)
        {
            _context = context;
            _uploader = uploader;
            _config = config;
            _notificationsRepo = notificationsRepo;
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
            var tags = await _context.Tags
                .Where(t => Input.Tags.Contains(t.Id))
                .ToListAsync();

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
            
            // Get a list of users that should receive notifications
            var notificationRecipients = await _context.Users
                .Where(u => u.Following.Any(a => a.Id == uid))
                .Select(u => u.Id)
                .ToListAsync();
            
            // Notify
            await _notificationsRepo.Create(ENotificationEvent.FollowedAuthorNewStory,
                notificationRecipients,
                "/Story",
                new { story.Id, story.Slug });

            return RedirectToPage("../Story", new {id = story.Id, slug = story.Slug});
        }
    }
}
