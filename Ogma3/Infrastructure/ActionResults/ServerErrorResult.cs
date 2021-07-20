using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Ogma3.Infrastructure.ActionResults
{
    [DefaultStatusCode(DefaultStatusCode)]
    public class ServerErrorResult : ObjectResult
    {
        private const int DefaultStatusCode = StatusCodes.Status500InternalServerError;
        
        public ServerErrorResult(object value) : base(value)
        {
            StatusCode = DefaultStatusCode;
        }
    }
}