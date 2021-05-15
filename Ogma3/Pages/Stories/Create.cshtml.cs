using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Notifications;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.CustomValidators;
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
        private readonly IMapper _mapper;

        public CreateModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig config, NotificationsRepository notificationsRepo, IMapper mapper)
        {
            _context = context;
            _uploader = uploader;
            _config = config;
            _notificationsRepo = notificationsRepo;
            _mapper = mapper;
        }

        public List<RatingDto> Ratings { get; set; }
        public List<TagDto> Genres { get; set; }
        public List<TagDto> ContentWarnings { get; set; }
        public List<TagDto> Franchises { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            Input ??= new InputModel();
            Ratings = await _context.Ratings
                .OrderBy(r => r.Order)
                .ProjectTo<RatingDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var tags = await _context.Tags
                .OrderBy(t => t.Name)
                .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            Genres = tags.Where(t => t.Namespace == ETagNamespace.Genre).ToList();
            ContentWarnings = tags.Where(t => t.Namespace == ETagNamespace.ContentWarning).ToList();
            Franchises = tags.Where(t => t.Namespace == ETagNamespace.Franchise).ToList();
            
            return Page();
        }

        [BindProperty] 
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Title { get; init; }
            public string Description { get; init; }
            public string Hook { get; init; }
            [DataType(DataType.Upload)]
            public IFormFile Cover { get; init; }
            public long Rating { get; init; }
            public List<long> Tags { get; set; }
        }
        
        public class InputModelValidation : AbstractValidator<InputModel>
        {
            public InputModelValidation()
            {
                RuleFor(i => i.Title)
                    .NotEmpty()
                    .Length(CTConfig.CStory.MinTitleLength, CTConfig.CStory.MaxTitleLength);
                RuleFor(i => i.Description)
                    .NotEmpty()
                    .Length(CTConfig.CStory.MinDescriptionLength, CTConfig.CStory.MaxDescriptionLength);
                RuleFor(i => i.Hook)
                    .NotEmpty()
                    .Length(CTConfig.CStory.MinHookLength, CTConfig.CStory.MaxHookLength);
                RuleFor(i => i.Cover)
                    .FileSmallerThan(CTConfig.CStory.CoverMaxWeight)
                    .FileHasExtension(new[] {".jpg", ".jpeg", ".png"});
                RuleFor(i => i.Rating).NotEmpty();
                RuleFor(i => i.Tags).NotEmpty();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return await OnGetAsync();
            
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
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
                RatingId = Input.Rating,
                Tags = tags
            };

            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();

            // Upload cover
            if (Input.Cover is {Length: > 0})
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
