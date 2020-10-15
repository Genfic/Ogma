using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Ogma3.Infrastructure.Attributes
{
    public enum TimeUnit
    {
        Minute = 60,
        Hour = 3600,
        Day = 86400
    }
 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        public TimeUnit TimeUnit { get; set; }
        public int Count { get; set; }
 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cache = filterContext.HttpContext.RequestServices.GetService<IMemoryCache>();
            var seconds = Convert.ToInt32(TimeUnit);

            var controllerActionDescriptor = (ControllerActionDescriptor) filterContext.ActionDescriptor;
            var key = string.Join(
                "-",
                seconds,
                filterContext.HttpContext.Request.Method,
                controllerActionDescriptor.ControllerName,
                controllerActionDescriptor.ActionName,
                filterContext.HttpContext.Request.Host.Host
            );
 
            // increment the cache value
            var cnt = 1;
            if (cache.Get(key) != null) {
                cnt = (int)cache.Get(key) + 1;
            }

            cache.Set(key, cnt, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(seconds),
                Priority = CacheItemPriority.Low,
            });

            if (cnt <= Count) return;
            
            filterContext.Result = new ContentResult
            {
                Content = "You are allowed to make only " + Count + " requests per " + TimeUnit.ToString().ToLower()
            };
            filterContext.HttpContext.Response.StatusCode = 429;
        }
    }
}