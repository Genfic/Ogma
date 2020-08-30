using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Areas.Admin.Pages.Documents
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Document> Docs { get; set; }
        public async void OnGetAsync()
        {
            Docs = await _context.Documents
                .Where(d => d.RevisionDate == null)
                .OrderBy(d => d.Slug)
                .AsNoTracking()
                .ToListAsync(); // TODO: Fix it being null
        }
    }
}