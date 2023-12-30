using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

[HtmlTargetElement("aside", Attributes = ForAttributeName)]
public class AsideTagHelper : TagHelper
{
	private const string ForAttributeName = "asp-for";

	[HtmlAttributeName(ForAttributeName)]
	public ModelExpression? For { get; set; }

	/// <inheritdoc />
	/// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);

		output.TagName = "aside";
		if (For is {} f) {
			output.Attributes.Add("data-for", f.Name.Replace('.', '_'));
		}
		output.Content.SetHtmlContent(childContent);
	}
}