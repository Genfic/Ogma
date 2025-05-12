using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

[HtmlTargetElement("img", Attributes = "cf", TagStructure = TagStructure.WithoutEndTag)]
[method: ActivatorUtilitiesConstructor]
public sealed class ImageTagHelper(
	IUrlHelperFactory urlHelperFactory,
	HtmlEncoder htmlEncoder
) : UrlResolutionTagHelper(urlHelperFactory, htmlEncoder)
{
	[HtmlAttributeName("cf")]
	public required bool Cf { get; set; }
	public required string Src { get; set; }
	public required int Width { get; set; }
	public required int Height { get; set; }

	[HtmlAttributeName("cf-height")]
	public required int? CfHeight { get; set; }

	[HtmlAttributeName("cf-width")]
	public required int? CfWidth { get; set; }

	public bool Eager { get; set; } = false;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(output);

		var src = Src.StartsWith("//")
			? $"https://genfic.net/cdn-cgi/image/h={CfHeight ?? Height},w={CfWidth ?? Width},format=auto/https:{Src}"
			: Src;

		output.Attributes.SetAttribute("width", Width);
		output.Attributes.SetAttribute("height", Height);
		output.Attributes.SetAttribute("src", src);

		if (!Eager)
		{
			output.Attributes.SetAttribute("loading", "lazy");
		}
	}
}
