using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services.TagHelpers
{
    public class DocumentLinkTagHelper : TagHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;

        public DocumentLinkTagHelper(ApplicationDbContext context, IHttpContextAccessor accessor, LinkGenerator generator)
        {
            _context = context;
            _accessor = accessor;
            _generator = generator;
        }

        public string GroupId { get; set; }
        public int? Version { get; set; } = null;
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            var guid = Guid.Parse(GroupId);

            var query = _context.Documents
                .Where(d => d.GroupId == guid);

            query = Version == null 
                ? query.Where(d => d.RevisionDate == null) 
                : query.Where(d => d.Version == Version);
                
            var doc = await query
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            var href = doc == null
                ? ""
                : _generator.GetUriByPage(_accessor.HttpContext, "./Docs", null, new{ id = doc.Id, slug = doc.Slug });

            output.Content.SetHtmlContent(content.GetContent());
            output.TagName = "a";
            output.AddClass("doc-link", NullHtmlEncoder.Default);
            output.Attributes.Add("href", href);
        }
    }
}