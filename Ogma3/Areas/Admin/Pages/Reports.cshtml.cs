using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;

namespace Ogma3.Areas.Admin.Pages
{
    public class Reports : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private const int PerPage = 50;

        public Reports(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public List<ReportDto> ReportsList { get; set; }
        public Pagination Pagination { get; set; }
        
        public async Task OnGetAsync([FromQuery] int page = 1)
        {
            ReportsList = await _context.Reports
                .OrderByDescending(r => r.ReportDate)
                .Paginate(page, PerPage)
                .ProjectTo<ReportDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            var count = await _context.Reports.CountAsync();

            Pagination = new Pagination
            {
                CurrentPage = page,
                ItemCount = count,
                PerPage = PerPage
            };
        }

        /// <summary>
        /// The comment system here is so incredibly complex it needs its own named handler lol
        /// Its purpose is to redirect to the content the given comment is attached to, and to scroll to said comment
        /// </summary>
        /// <param name="id">ID of the comment</param>
        /// <returns></returns>
        public async Task<ActionResult> OnGetComment(long id)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    CommentIds = c.CommentsThread.Comments.Select(e => e.Id),
                    c.CommentsThread.User.UserName,
                    c.CommentsThread.BlogpostId,
                    c.CommentsThread.ChapterId,
                    c.CommentsThread.ClubThreadId,
                    c.CommentsThread.ClubThread
                })
                .FirstOrDefaultAsync();

            if (comment is null) return NotFound();

            // Get the ordinal number of the comment within the thread.
            // +1 because they're 1-indexed.
            var order = comment.CommentIds.ToList().IndexOf(id) + 1;
            
            // Figure out the redirect
            if (comment.UserName is not null)
                return RedirectToPage("/User/Index", null, new { Name = comment.UserName }, $"comment-{order}");
            if (comment.BlogpostId is not null)
                return RedirectToPage("/Blog/Post", null, new { Id = comment.BlogpostId }, $"comment-{order}");
            if (comment.ChapterId is not null)
                return RedirectToPage("/Chapter",  null,new {Id = comment.ChapterId}, $"comment-{order}");
            if (comment.ClubThreadId is not null)
                return RedirectToPage("/Club/Forums/Details", null, new { comment.ClubThread.ClubId, ThreadId = comment.ClubThreadId}, $"comment-{order}");

            return NotFound();

        }
    }
}