using System;
using System.Linq;
using System.Linq.Expressions;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Data.Mappings
{
    public static class ShelfMappings
    {
        public static Expression<Func<Shelf, ShelfDto>> ToShelfDto(long? bookId = null) => shelf => new ShelfDto
        {
            Id = shelf.Id,
            Name = shelf.Name,
            Description = shelf.Description,
            IsDefault = shelf.IsDefault,
            IsPublic = shelf.IsPublic,
            IsQuick = shelf.IsQuickAdd,
            Color = shelf.Color,
            StoriesCount = shelf.Stories != null ? shelf.Stories.Count : 0,
            IconName = shelf.Icon != null ? shelf.Icon.Name : null,
            IconId = shelf.Icon != null ? shelf.Icon.Id : (long?) null,
            DoesContainBook = bookId == null
                ? null
                : shelf.Stories != null 
                    ? shelf.Stories.Any(ss => ss.Id == bookId) 
                    : (bool?) null
        };
    }
}