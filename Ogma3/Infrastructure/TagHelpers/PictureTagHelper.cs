using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Ogma3.Infrastructure.TagHelpers;

[HtmlTargetElement(
    "picture",
    Attributes = AppendVersionAttributeName + "," + SrcAttributeName)]
public class PictureTagHelper : UrlResolutionTagHelper
{
    private const string AppendVersionAttributeName = "asp-append-version";
    private const string SrcAttributeName = "src";

    internal IFileVersionProvider FileVersionProvider { get; private set; }

    [ActivatorUtilitiesConstructor]
    public PictureTagHelper(
        IUrlHelperFactory urlHelperFactory,
        HtmlEncoder htmlEncoder,
        IFileVersionProvider fileVersionProvider
    ) : base(urlHelperFactory, htmlEncoder)
    {
        FileVersionProvider = fileVersionProvider;
    }

    [HtmlAttributeName(SrcAttributeName)]
    public string Src { get; set; }
        
    [HtmlAttributeName(AppendVersionAttributeName)]
    public bool AppendVersion { get; set; }
        
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Eager { get; set; } = false;
    public string[] SourceFormats { get; set; } = Array.Empty<string>();
    public string Alt { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (output == null) throw new ArgumentNullException(nameof(output));
            
        output.CopyHtmlAttribute(SrcAttributeName, context);
        ProcessUrlAttribute(SrcAttributeName, output);
            
        var bareUrl = Src[..Src.LastIndexOf('.')];
            
        if (AppendVersion)
        {
            EnsureFileVersionProvider();
        }

        foreach (var format in SourceFormats)
        {
            var formatUrl = $"{bareUrl}.{format}";
            var finalUrl = AppendVersion
                ? FileVersionProvider.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, formatUrl)
                : formatUrl;
            output.Content.AppendHtml($@"<source type=""image/{format}"" srcset=""{finalUrl}"" />");
        }
            
        var url = AppendVersion
            ? FileVersionProvider.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, Src)
            : Src;

        output.Content.AppendHtml(!Eager
            ? $@"<img src=""{url}"" alt=""{Alt}"" width=""{Width}"" height=""{Height}"">"
            : $@"<img src=""{url}"" alt=""{Alt}"" width=""{Width}"" height=""{Height}"" loading=""lazy"">");
            
        output.Attributes.Clear();
    }
        
    private void EnsureFileVersionProvider()
    {
        FileVersionProvider ??= ViewContext.HttpContext.RequestServices.GetRequiredService<IFileVersionProvider>();
    }
}