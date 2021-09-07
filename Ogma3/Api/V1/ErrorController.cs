using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ErrorController))]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Result> OnGet([FromQuery] int? code)
        {
            var text = code.HasValue 
                ? ReasonPhrases.GetReasonPhrase((int) code) 
                : "Unknown Error";
            return new JsonResult(new Result(code, text))
            {
                StatusCode = code ?? 0
            };
        }

        public sealed record Result(int? Code, string Reason);
    }
}