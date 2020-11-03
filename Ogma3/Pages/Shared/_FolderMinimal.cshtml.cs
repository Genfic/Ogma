using System.Collections.Generic;
using Ogma3.Data.DTOs;

namespace Ogma3.Pages.Shared
{
    public class FolderMinimal
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public string Name { get; set; }
        public int StoriesCount { get; set; }
    }
}