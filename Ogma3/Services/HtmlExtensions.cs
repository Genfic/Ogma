using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ogma3.Services
{
    public static class HtmlExtensions
    {
        public static HtmlString Resource(this IHtmlHelper htmlHelper, Func<object, HelperResult> template, string type = "js")
        {
            if (htmlHelper.ViewContext.HttpContext.Items[type] != null) ((List<Func<object, HelperResult>>)htmlHelper.ViewContext.HttpContext.Items[type]).Add(template);
            else htmlHelper.ViewContext.HttpContext.Items[type] = new List<Func<object, HelperResult>> { template };

            return new HtmlString(string.Empty);
        }

        public static HtmlString RenderResources(this IHtmlHelper htmlHelper, string type = "js")
        {
            if (htmlHelper.ViewContext.HttpContext.Items[type] != null)
            {
                var resources = (List<Func<object, HelperResult>>)htmlHelper.ViewContext.HttpContext.Items[type];

                foreach (var resource in resources.Where(resource => resource != null))
                {
                    htmlHelper.ViewContext.Writer.Write(resource(null));
                }
            }

            return new HtmlString(string.Empty);
        }
    }
}