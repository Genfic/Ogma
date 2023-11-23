using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Areas.Admin.TagHelpers;

public class NavHelper : TagHelper
{

	[HtmlAttributeNotBound]
	[ViewContext]
	public required ViewContext ViewContext { get; set; }

	public required string Page { get; set; }
	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var activePage = ViewContext.ViewData["ActivePage"] as string
		                 ?? Path.GetFileNameWithoutExtension(ViewContext.ActionDescriptor.DisplayName);
		
		var css = string.Equals(activePage, Page, StringComparison.OrdinalIgnoreCase) ? "active" : null;

		output.TagName = "li";
		output.Attributes.Add("class", $"nav-item {css}");
	}
}