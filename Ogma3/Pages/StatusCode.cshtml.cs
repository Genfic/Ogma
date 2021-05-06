using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Ogma3.Pages
{
    public class StatusCodeModel : PageModel
    {
        public string OriginalUrl { get; private set; }
        public int Code { get; private set; }
        public string Name { get; private set; }
        public string Text { get; private set; }
        
        public void OnGet(int? code)
        {
            if (code is not null)
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

            Text = code switch
            {
                >= 100 and < 200 => "It's just an information code, but it should not have shown up...",
                >= 200 and < 300 => "That means everything is fine! And yet, you somehow got to this error page...",
                >= 300 and < 400 => "You got redirected, which means you should see the target page, not this error message...",
                >= 400 and < 500 => "That's on your end! You're trying to access something that doesn't exist, or something you're not authorized to access.",
                >= 500 and < 600 => "That's our bad! Something went wrong on the server. Hopefully nothing too hard to fix...",
                _ => "I have no clue what this error is, nor how you managed to get it."
            };
        }

    }
}