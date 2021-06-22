using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class CdnPictureTagHelper : TagHelper
    {
        private readonly OgmaConfig _config;

        public CdnPictureTagHelper(OgmaConfig config)
        {
            _config = config;
        }
        
        public string Src { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Eager { get; set; } = false;
        public string[] SourceFormats { get; set; }
        public string? Buster { get; set; } = null;
        public string Alt { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var url = _config.Cdn + Src.Trim('/');
            var bareUrl = url[..url.LastIndexOf('.')];

            if (!string.IsNullOrEmpty(Buster)) url += $"?v={Buster}";
            
            output.TagName = "picture";

            foreach (var format in SourceFormats)
            {
                var fullUrl = $"{bareUrl}.{format}";
                output.Content.AppendHtml($@"<source type=""image/{format}"" srcset=""{fullUrl}"" />");
            }


            output.Content.AppendHtml(!Eager
                ? $@"<img src=""{url}"" alt=""{Alt}"" width=""{Width}"" height=""{Height}"">"
                : $@"<img src=""{url}"" alt=""{Alt}"" width=""{Width}"" height=""{Height}"" loading=""lazy"">");
        }
    }
}