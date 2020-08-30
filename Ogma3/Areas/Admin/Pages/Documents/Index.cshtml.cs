using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.Documents
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public int? Id { get; set; }
            
            [Required]
            public string Title { get; set; }
            
            [Required]
            public string Body { get; set; }
        }

        public IEnumerable<Document> Docs { get; set; }
        public async void OnGetAsync()
        {
            Docs = await _context.Documents
                .Where(d => d.RevisionDate == null)
                .OrderBy(d => d.Slug)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var oldVersion = await _context.Documents
                .Where(d => d.RevisionDate == null)
                .Where(d => d.Id == Input.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (oldVersion == null)
            {
                await _context.Documents.AddAsync(new Document
                {
                    GroupId = Guid.NewGuid(),
                    Title = Input.Title,
                    Slug = Input.Title.Friendlify(),
                    Body = Input.Body,
                    Version = 1,
                    CreationTime = DateTime.Now,
                    RevisionDate = null
                });
            }
            else
            {
                var now = DateTime.Now;
                await _context.Documents.AddAsync(new Document
                {
                    GroupId = oldVersion.GroupId,
                    Title = Input.Title.IsNullOrEmpty() ? Input.Title : oldVersion.Title,
                    Slug = Input.Title.IsNullOrEmpty() ? Input.Title.Friendlify() : oldVersion.Slug,
                    Body = Input.Body.IsNullOrEmpty() ? Input.Body : oldVersion.Body,
                    Version = oldVersion.Version += 1,
                    CreationTime = now,
                    RevisionDate = null
                });
                
                oldVersion.RevisionDate = now;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToPage("./Documents");
        }
    }
}