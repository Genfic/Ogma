using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    public class BrowseModel : PageModel
    {
        private ApplicationDbContext _context;
        
        public IList<Story> Stories { get; set; }
        public int StoriesCount { get; set; }
        
        public int PageNumber { get; set; }
        public readonly int PerPage = 25;
        public EStorySortingOptions SortBy { get; set; }
        public string SearchBy { get; set; }
        public List<Tag> Tags { get; set; }
        public Rating Rating { get; set; }
        
        public BrowseModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async void OnGetAsync(
            [FromQuery] string search, 
            [FromQuery] EStorySortingOptions sort,
            [FromQuery] Rating rating,
            [FromQuery] int page = 1
        )
        {
            SearchBy = search;
            SortBy = sort;
            Rating = rating;
            PageNumber = page;
            
            var query = _context.Stories
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage);
            
            // Search by title
            if (!search.IsNullOrEmpty())
            {
                query = query
                    .Where(s => EF.Functions.Like(s.Title.ToUpper(), $"%{search.Trim().ToUpper()}%"));
            }
            
            // Search by rating
            if (rating != null)
            {
                query = query
                    .Where(s => s.Rating == rating);
            }
            
            // Sort
            query = sort switch
            {
                EStorySortingOptions.TitleAscending  => query.OrderBy(s => s.Title),
                EStorySortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
                EStorySortingOptions.DateAscending   => query.OrderBy(s => s.ReleaseDate),
                EStorySortingOptions.DateDescending  => query.OrderByDescending(s => s.ReleaseDate),
                EStorySortingOptions.WordsAscending  => query.OrderBy(s => s.WordCount),
                EStorySortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
                _ => query.OrderByDescending(s => s.WordCount)
            };
        }

    }
}