using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class CdnImgTagHelper : TagHelper
    {
        private readonly OgmaConfig _ogmaConfig;
        public CdnImgTagHelper(OgmaConfig ogmaConfig)
        {
            _ogmaConfig = ogmaConfig;
        }

        public string Src { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool Eager { get; set; } = false;

        public string? Buster { get; set; } = null;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var src = string.IsNullOrEmpty(Src) ? "ph-250.png" : Src;
            var url = _ogmaConfig.Cdn + src.Trim('/');

            if (Width.HasValue) output.Attributes.SetAttribute("width", Width ?? Height);
            if (Height.HasValue) output.Attributes.SetAttribute("height", Height ?? Width);

            if (!string.IsNullOrEmpty(Buster)) url += $"?v={Buster}";
            
            output.TagName = "img";
            output.Attributes.SetAttribute("src", url);

            if (!Eager)
            {
                output.Attributes.SetAttribute("loading", "lazy");
            }
        }
    }
}