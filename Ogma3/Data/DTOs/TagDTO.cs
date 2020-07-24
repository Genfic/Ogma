using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Data.DTOs
{
    public class TagDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Namespace { get; set; }
        public string Color { get; set; }
        public string Rgba { get; set; }

        public static TagDTO FromTag(Tag tag)
        {
            var dto = new TagDTO
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                Namespace = tag.Namespace?.Name,
                Color = tag.Namespace?.Color
            };
            
            if (tag.Namespace?.Color != null)
            {
                var color = tag.Namespace.Color.ParseHexColor();
                var csColor = System.Drawing.Color.FromArgb(150, color.R, color.G, color.B).ToCommaSeparatedCss();
                dto.Rgba = $"rgba({csColor})";
            }

            return dto;
        }
    }
}