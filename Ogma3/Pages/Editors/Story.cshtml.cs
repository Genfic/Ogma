using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Services.Attributes;
using Utils;

namespace Ogma3.Pages.Editors
{
    [Authorize]
    public class StoryEditorModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IB2Client _b2Client;
        private readonly IConfiguration _config;

        public StoryEditorModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IB2Client b2Client,
            IConfiguration config
        )
        {
            _context = context;
            _userManager = userManager;
            _b2Client = b2Client;
            _config = config;
        }

        public List<Rating> Ratings { get; set; }
        
        [BindProperty]
        public int[] Tags { get; set; }
        public SelectList TagOptions { get; set; }

        public async Task OnGetAsync()
        {
            Ratings = await _context.Ratings.ToListAsync();
//            Tags = await _context.Tags.ToListAsync();
            TagOptions = new SelectList(await _context.Tags.ToListAsync(), nameof(Tag.Id), nameof(Tag.Name));
        }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(
                CTConfig.Story.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Story.MinTitleLength
            )]
            public string Title { get; set; }

            [Required]
            [StringLength(
                CTConfig.Story.MaxDescriptionLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Story.MinDescriptionLength
            )]
            public string Description { get; set; }

            [Required]
            [StringLength(
                CTConfig.Story.MaxHookLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Story.MinHookLength
            )]
            public string Hook { get; set; }

            [DataType(DataType.Upload)]
            [MaxFileSize(CTConfig.Story.CoverMaxWeight)]
            [AllowedExtensions(new[] {".jpg", ".jpeg", ".png"})]
            public IFormFile Cover { get; set; }

            [Required] 
            public Rating Rating { get; set; }

            [Required] 
            public List<Tag> Tags { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
//                string cover;
//                string coverId;
//                
//                // Handle cover upload
//                if (Input.Cover != null && Input.Cover.Length > 0)
//                {
//                    string safeTitle = Input.Title.Friendlify();
//                    
//                    var ext = Input.Cover.FileName.Split('.').Last();
//                    var fileName = $"covers/{user.Id}.{ext}";
//                
//                    // Upload new one
//                    await using var ms = new MemoryStream();
//                    Input.Cover.CopyTo(ms);
//                    var file = await _b2Client.Files.Upload(ms.ToArray(), fileName);
//
//                    coverId = file.FileId;
//                    cover = _config["cdn"] + fileName;
//                }

                var story = new Story
                {
                    Author = user,
                    Title = Input.Title,
                    Slug = Input.Title.Friendlify(),
                    Description = Input.Description,
                    Hook = Input.Hook,
                    Rating = Input.Rating,
                };

                _context.Stories.Add(story);
                await _context.SaveChangesAsync();

                // Add tags and associations
//                foreach (var tag in Input.Tags)
//                {
//                    await _context.StoryTags.AddAsync(new StoryTag
//                    {
//                        
//                    });
//                }
                return null;
            }
            else
            {
                Console.WriteLine("===========================================");
                foreach (var msv in ModelState.Values)
                {
                    foreach (var error in msv.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                Console.WriteLine("===========================================");
                return RedirectToPage();
            }
        }
    }
}