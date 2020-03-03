using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class ShelfFromApiDTO
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public IEnumerable<Story> Stories;

        public static ShelfFromApiDTO FromShelf(Shelf shelf)
            => new ShelfFromApiDTO
            {
                Name = shelf.Name,
                IsDefault = shelf.IsDefault,
                IsPublic = shelf.IsPublic,
                Stories = shelf.Stories
            };
    }
}