using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Data.DTOs
{
    public class TagDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string NamespaceName { get; set; }
        public string NamespaceColor { get; set; }
        // public string Rgba { get; set; }

        public static TagDto FromTag(Tag tag)
        {
            var dto = new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                NamespaceName = tag.Namespace?.Name,
                NamespaceColor = tag.Namespace?.Color
            };

            return dto;
        }
    }
}