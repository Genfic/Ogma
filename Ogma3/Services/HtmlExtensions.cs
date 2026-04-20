using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ogma3.Services;

public static class HtmlExtensions
{
	extension(IHtmlHelper htmlHelper)
	{
		/// <summary>
		/// Register a resource of a given `type` to be rendered
		/// </summary>
		/// <param name="template"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public HtmlString Resource(Func<object?, HelperResult> template, string type = "js")
		{
			if (htmlHelper.ViewContext.HttpContext.Items[type] is {} item)
			{
				((List<Func<object?, HelperResult>?>)item).Add(template);
			}
			else
			{
				htmlHelper.ViewContext.HttpContext.Items[type] = new List<Func<object?, HelperResult>> { template };
			}

			return new HtmlString(string.Empty);
		}
		/// <summary>
		/// Define a spot where resources of a given `type` should be rendered
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public HtmlString RenderResources(string type = "js")
		{
			if (htmlHelper.ViewContext.HttpContext.Items[type] is not {} item) return new HtmlString(string.Empty);

			var resources = (List<Func<object?, HelperResult>?>)item;
			foreach (var resource in resources.OfType<Func<object?, HelperResult>>())
			{
				htmlHelper.ViewContext.Writer.Write(resource(null));
			}

			return new HtmlString(string.Empty);
		}
	}

}