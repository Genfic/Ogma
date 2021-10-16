#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Blog;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepo;

    public DetailsModel(UserRepository userRepo, ApplicationDbContext context)
    {
        _userRepo = userRepo;
        _context = context;
    }

    public Details? Blogpost { get; private set; }
    public ProfileBar ProfileBar { get; private set; } = null!;

    public bool IsUnavailable { get; private set; }
        
    public class Details
    {
        public long Id { get; init; }
        public long AuthorId { get; init; }
        public string Title { get; init; } = null!;
        public string Slug { get; init; } = null!;
        public DateTime? PublicationDate { get; init; }
        public string Body { get; init; } = null!;
        public IEnumerable<string> Hashtags { get; init; } = null!;
        public CommentsThreadDto CommentsThread { get; init; } = null!;
        public int CommentsCount { get; init; }
        public ChapterMinimal? AttachedChapter { get; init; }
        public StoryMinimal? AttachedStory { get; init; }
        public ContentBlockCard? ContentBlock { get; init; }
    }

    public async Task<IActionResult> OnGetAsync(long id, string? slug)
    {
        var uid = User.GetNumericId();

        Blogpost = await _context.Blogposts
            .TagWith($"Get blogpost -> {id}")
            .Where(b => b.Id == id)
            .Where(b => b.PublicationDate != null || b.AuthorId == uid)
            .Where(b => b.ContentBlockId == null || b.AuthorId == uid || User.IsStaff())
            .Select(MapDetails)
            .FirstOrDefaultAsync();

        if (Blogpost is null) return NotFound();

        if (Blogpost.AttachedChapter is not null && Blogpost.AttachedChapter.PublicationDate is null)
        {
            IsUnavailable = true;
        }
        else if (Blogpost.AttachedStory is not null && Blogpost.AttachedStory.PublicationDate is null)
        {
            IsUnavailable = true;
        }

        Blogpost.CommentsThread.Type = nameof(Data.Blogposts.Blogpost);
            
        ProfileBar = await _userRepo.GetProfileBar(Blogpost.AuthorId);
            
        return Page();
    }

    private static Expression<Func<Blogpost, Details>> MapDetails => b => new Details
    {
        Id = b.Id,
        AuthorId = b.AuthorId,
        Title = b.Title,
        Slug = b.Slug,
        Body = b.Body,
        Hashtags = b.Hashtags,
        PublicationDate = b.PublicationDate,
        CommentsCount = b.CommentsThread.CommentsCount,
        CommentsThread = new CommentsThreadDto
        {
            Id = b.CommentsThread.Id,
            Type = nameof(Data.Blogposts.Blogpost),
            LockDate = b.CommentsThread.LockDate
        },
        ContentBlock = b.ContentBlock == null ? null : new ContentBlockCard
        {
            Reason = b.ContentBlock.Reason,
            DateTime = b.ContentBlock.DateTime,
            IssuerUserName = b.ContentBlock.Issuer.UserName
        },
        AttachedChapter = b.AttachedChapter == null ? null : new ChapterMinimal
        {
            Id = b.AttachedChapter.Id,
            Title = b.AttachedChapter.Title,
            Slug = b.AttachedChapter.Slug,
            PublicationDate = b.AttachedChapter.PublicationDate,
            StoryTitle = b.AttachedChapter.Story.Title,
            StoryAuthorUserName = b.AttachedChapter.Story.Author.UserName
        },
        AttachedStory = b.AttachedStory == null ? null : new StoryMinimal
        {
            Id = b.AttachedStory.Id,
            Title = b.AttachedStory.Title,
            Slug = b.AttachedStory.Slug,
            PublicationDate = b.AttachedStory.PublicationDate,
            AuthorUserName = b.AttachedStory.Author.UserName
        }
    };

}