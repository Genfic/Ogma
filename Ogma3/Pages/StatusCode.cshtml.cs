using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Ogma3.Pages
{
    public class StatusCodeModel : PageModel
    {
        public string OriginalUrl { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        
        public void OnGet(int? code)
        {
            if (code.HasValue)
            {
                Code = (int) code;
                Name = ReasonPhrases.GetReasonPhrase((int) code);
            }
            else
            {
                Code = 0;
                Name = "Unknown Error";
            }
            
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCodeReExecuteFeature != null)
            {
                OriginalUrl = $"{Request.Scheme}://{Request.Host}"
                    + statusCodeReExecuteFeature.OriginalPathBase
                    + statusCodeReExecuteFeature.OriginalPath
                    + statusCodeReExecuteFeature.OriginalQueryString;
                
            }

            if (code >= 100 && code < 200)
            {
                Text = "It's just an information code, but it should not have shown up...";
            } 
            else if (code >= 200 && code < 300)
            {
                Text = "That means everything is fine! And yet, you somehow got to this error page...";
            } 
            else if (code >= 300 && code < 400)
            {
                Text = "You got redirected, which means you should see the target page, not this error message...";
            } 
            else if (code >= 400 && code < 500)
            {
                Text = "That's on your end! You're trying to access something that doesn't exist, or something you're not authorized to access.";
            } 
            else if (code >= 500 && code < 600)
            {
                Text = "That's our bad! Something went wrong on the server. Hopefully nothing too hard to fix...";
            }
            else
            {
                Text = "I have no clue what this error is, nor how you managed to get it.";
            }
        }

    }
}