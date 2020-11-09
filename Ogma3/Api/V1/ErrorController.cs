using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ErrorController))]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult OnGet([FromQuery] int? code)
        {
            var text = code.HasValue 
                ? ReasonPhrases.GetReasonPhrase((int) code) 
                : "Unknown Error";
            return new JsonResult(new { Code = code, Reason = text })
            {
                StatusCode = code ?? 0
            };
        }
    }
}