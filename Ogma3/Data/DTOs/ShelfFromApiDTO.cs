using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Data.DTOs
{
    public class ShelfFromApiDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public bool IsQuick { get; set; }
        public string Color { get; set; }
        public IEnumerable<Story> Stories;
        public int Count { get; set; }
        public string? Icon { get; set; }
        public long? IconId { get; set; }
        public bool? DoesContainBook { get; set; }

        public static ShelfFromApiDTO FromShelf(Shelf shelf, long? bookId = null) =>
            new ShelfFromApiDTO
            {
                Id = shelf.Id,
                Name = shelf.Name,
                Description = shelf.Description,
                IsDefault = shelf.IsDefault,
                IsPublic = shelf.IsPublic,
                IsQuick = shelf.IsQuickAdd,
                Color = shelf.Color,
                Stories = shelf.Stories,
                Count = shelf.ShelfStories.Count,
                Icon = shelf.Icon?.Name,
                IconId = shelf.Icon?.Id,
                DoesContainBook = bookId == null
                    ? (bool?) null
                    : shelf.ShelfStories.Any(ss => ss.StoryId == bookId)
            };
    }
}