using System.Collections.Generic;

namespace Ogma3.Pages.Shared
{
    public class BookshelfDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerUserName { get; set; }
        public bool IsPublic { get; set; }
        public string Color { get; set; }
        public string IconName { get; set; }
        public ICollection<StoryCard> Stories { get; set; }
    }
}