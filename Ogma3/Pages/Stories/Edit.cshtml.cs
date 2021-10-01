using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Stories;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ImageUploader _uploader;
    private readonly OgmaConfig _ogmaConfig;
    private readonly IMapper _mapper;

    public EditModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig ogmaConfig, IMapper mapper)
    {
        _context = context;
        _uploader = uploader;
        _ogmaConfig = ogmaConfig;
        _mapper = mapper;
    }
        
    public List<RatingDto> Ratings { get; private set; }
    public List<TagDto> Genres { get; private set; }
    public List<TagDto> ContentWarnings { get; private set; }
    public List<TagDto> Franchises { get; private set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        // Get logged in user
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        // Get story to edit and make sure author matches logged in user
        Input = await _context.Stories
            .Where(s => s.Id == id)
            .Where(s => s.AuthorId == uid)
            .Select(story => new InputModel
            {
                Id = story.Id,
                Title = story.Title,
                Description = story.Description,
                Hook = story.Hook,
                Rating = story.Rating.Id,
                Tags = story.Tags.Select(st => st.Id).ToList(),
                Status = story.Status,
                Published = story.PublicationDate != null
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (Input is null) return NotFound();
            
        // Fill Ratings dropdown
        await Hydrate();
        return Page();
    }

    [BindProperty] 
    public InputModel Input { get; set; }

    public class InputModel
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Hook { get; init; }

        [DataType(DataType.Upload)]
        public IFormFile Cover { get; init; }
        public long Rating { get; init; }
        public EStoryStatus Status { get; init; }
        public List<long> Tags { get; init; }
        public bool Published { get; init; }
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

    public async Task<IActionResult> OnPostAsync(long id)
    {
        if (!ModelState.IsValid)
        {
            await Hydrate();
            return Page();
        }

        var tags = await _context.Tags
            .Where(t => Input.Tags.Contains(t.Id))
            .ToListAsync();

        // Get logged in user
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
                
        // Get the story and make sure the logged-in user matches author
        var story = await _context.Stories
            .Include(s => s.Tags)
            .Include(s => s.Rating)
            .FirstOrDefaultAsync(s => s.Id == id && s.AuthorId == uid);
                
        // 404 if none found
        if (story is null) return NotFound();
                
        // Check if it can be published
        if (story.PublicationDate is null && Input.Published && story.ChapterCount <= 0)
        {
            ModelState.AddModelError("", "You cannot publish a story with no chapters");
            await Hydrate();
            return Page();
        }

        // Update story
        story.Title = Input.Title;
        story.Slug = Input.Title.Friendlify();
        story.Description = Input.Description;
        story.Hook = Input.Hook;
        story.Rating = await _context.Ratings.FindAsync(Input.Rating);
        story.Tags = tags;
        story.Status = Input.Status;
        story.PublicationDate = Input.Published ? DateTime.Now : null;
                
        _context.Update(story);
        await _context.SaveChangesAsync();
                
        // Handle cover upload
        if (Input.Cover is {Length: > 0})
        {
            // Upload cover
            var file = await _uploader.Upload(
                Input.Cover, 
                "covers", 
                story.Id.ToString(),
                _ogmaConfig.StoryCoverWidth,
                _ogmaConfig.StoryCoverHeight
            );
            story.CoverId = file.FileId;
            story.Cover = Path.Join(_ogmaConfig.Cdn, file.Path);
            // Final save
            await _context.SaveChangesAsync();
        }
                
        return RedirectToPage("../Story", new { id = story.Id, slug = story.Slug });
    }

    private async Task Hydrate()
    {
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
    }
}