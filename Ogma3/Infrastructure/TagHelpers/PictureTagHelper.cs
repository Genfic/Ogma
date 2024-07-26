using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

[HtmlTargetElement(
	"picture",
	Attributes = AppendVersionAttributeName + "," + SrcAttributeName)]
[method: ActivatorUtilitiesConstructor]
public class PictureTagHelper(
	IUrlHelperFactory urlHelperFactory,
	HtmlEncoder htmlEncoder,
	IFileVersionProvider fileVersionProvider
) : UrlResolutionTagHelper(urlHelperFactory, htmlEncoder)
{
	private const string AppendVersionAttributeName = "asp-append-version";
	private const string SrcAttributeName = "src";
	
	private IFileVersionProvider? FileVersionProvider { get; set; } = fileVersionProvider;

	[HtmlAttributeName(SrcAttributeName)]
	public required string Src { get; set; }

	[HtmlAttributeName(AppendVersionAttributeName)]
	public bool AppendVersion { get; set; }

	public int Width { get; set; }
	public int Height { get; set; }
	public bool Eager { get; set; } = false;
	public string[] SourceFormats { get; set; } = [];
	public string Alt { get; set; } = "";

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(output);

		output.CopyHtmlAttribute(SrcAttributeName, context);
		ProcessUrlAttribute(SrcAttributeName, output);

		var bareUrl = Src[..Src.LastIndexOf('.')];

		IFileVersionProvider? fvp = null;
		if (AppendVersion)
		{
			fvp = FileVersionProvider ?? ViewContext.HttpContext.RequestServices.GetRequiredService<IFileVersionProvider>();
		}

		foreach (var format in SourceFormats)
		{
			var formatUrl = $"{bareUrl}.{format}";
			var finalUrl = fvp is not null
				? fvp.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, formatUrl)
				: formatUrl;
			output.Content.AppendHtml($"""<source type="image/{format}" srcset="{finalUrl}" />""");
		}

		var url = fvp is not null
			? fvp.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, Src)
			: Src;

		output.Content.AppendHtml(!Eager
			? $"""<img src="{url}" alt="{Alt}" width="{Width}" height="{Height}">"""
			: $"""<img src="{url}" alt="{Alt}" width="{Width}" height="{Height}" loading="lazy">""");

		output.Attributes.Clear();
	}
}