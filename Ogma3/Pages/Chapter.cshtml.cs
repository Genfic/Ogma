using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages
{
    public class ChapterModel : PageModel
    {
        private readonly ChaptersRepository _chaptersRepo;

        public ChapterModel(ChaptersRepository chaptersRepo)
        {
            _chaptersRepo = chaptersRepo;
        }
        
        public ChapterDetails Chapter { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Chapter = await _chaptersRepo.GetChapterDetails(id);

            if (Chapter == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
