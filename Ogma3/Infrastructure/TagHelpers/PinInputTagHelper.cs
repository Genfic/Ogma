using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

[HtmlTargetElement("pin-input")]
public sealed class PinInputTagHelper(IHtmlGenerator generator) : TagHelper
{
    private const int MaxDigits = 6;

    [HtmlAttributeName("asp-for")]
    public required ModelExpression For { get; set; }

    [ViewContext, HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }

    private static readonly HelperResult Script = new(static async writer
	    => await writer.WriteAsync("""<script src="/js/pin-input.js" type="module"></script>"""));

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "pin-input");
        output.Attributes.SetAttribute("data-max-digits", MaxDigits);

        var label = generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, labelText: null, htmlAttributes: null);
        output.Content.AppendHtml(label);

        var boxes = new TagBuilder("div");
        boxes.AddCssClass("boxes");
        for (var i = 0; i < MaxDigits; i++)
        {
            var box = new TagBuilder("input")
            {
	            Attributes =
	            {
		            ["type"] = "text",
		            ["inputmode"] = "numeric",
		            ["pattern"] = "[0-9●]",
		            ["maxlength"] = "1",
		            ["autocomplete"] = "off",
		            ["autocapitalize"] = "off",
		            ["spellcheck"] = "false",
		            ["aria-label"] = $"Digit {i} of {MaxDigits}",
		            ["data-index"] = i.ToString(),
	            },
            };
            box.AddCssClass("pin-box active-border");
            boxes.InnerHtml.AppendHtml(box);
        }
        output.Content.AppendHtml(boxes);

        var hidden = generator.GenerateHidden(ViewContext, For.ModelExplorer, For.Name, For.Model, useViewData: false, htmlAttributes: new { @class = "pin-value" });
        output.Content.AppendHtml(hidden);

        var validation = generator.GenerateValidationMessage(ViewContext, For.ModelExplorer, For.Name, message: null, tag: null, htmlAttributes: new { @class = "text-danger" });
        output.Content.AppendHtml(validation);

        // push script
        var items = ViewContext.HttpContext.Items;
        if (items["js"] is List<Func<object?, HelperResult>> list)
        {
	        list.Add(_ => Script);
        }
        else
        {
	        items["js"] = new List<Func<object?, HelperResult>> { _ => Script };
        }
    }
}