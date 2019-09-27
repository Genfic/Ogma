using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ogma3.Areas.Admin.Pages
{
    public class ManageNavPages
    {
        public static string Index => "Index";
        
        public static string Tags => "Tags";
        
        public static string Categories => "Categories";
        
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        
        public static string TagsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Tags);
        
        public static string CategoriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Categories);
        
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}