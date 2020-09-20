using System;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;

namespace Ogma3.Services.TagHelpers
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
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var src = Src.IsNullOrEmpty() ? "ph-250.png" : Src;
            var url = _ogmaConfig.Cdn + src.Trim('/');

            if (Width.HasValue) output.Attributes.SetAttribute("width", Width);
            if (Height.HasValue) output.Attributes.SetAttribute("height", Height);
            
            output.TagName = "img";
            output.Attributes.SetAttribute("src", url);
        }
    }
}