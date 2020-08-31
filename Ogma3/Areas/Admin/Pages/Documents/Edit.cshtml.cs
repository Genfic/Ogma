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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public Guid Id { get; set; }
            
            [Required]
            public string Title { get; set; }
            
            [Required]
            public string Body { get; set; }
        }

        public Document Doc { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Doc = await _context.Documents
                .Where(d => d.GroupId == id)
                .Where(d => d.RevisionDate == null)
                .FirstOrDefaultAsync();

            if (Doc == null)
            {
                return NotFound();
            }
            
            Input = new InputModel
            {
                Id = Doc.GroupId,
                Title = Doc.Title,
                Body = Doc.Body
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var oldVersion = await _context.Documents
                .Where(d => d.RevisionDate == null)
                .Where(d => d.GroupId == Input.Id)
                .FirstOrDefaultAsync();

            var now = DateTime.Now;
            
            await _context.Documents.AddAsync(new Document
            {
                GroupId = oldVersion.GroupId,
                Title   = Input.Title.IsNullOrEmpty() ? oldVersion.Title : Input.Title,
                Slug    = Input.Title.IsNullOrEmpty() ? oldVersion.Slug  : Input.Title.Friendlify(),
                Body    = Input.Body.IsNullOrEmpty()  ? oldVersion.Body  : Input.Body,
                Version = oldVersion.Version + 1,
                CreationTime = now,
                RevisionDate = null
            });
            
            oldVersion.RevisionDate = now;
            
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}