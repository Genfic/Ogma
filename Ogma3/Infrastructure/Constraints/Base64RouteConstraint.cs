using System.Buffers.Text;

namespace Ogma3.Infrastructure.Constraints;

public sealed class Base64RouteConstraint : IRouteConstraint
{
	public bool Match(
		HttpContext? httpContext,
		IRouter? route,
		string routeKey,
		RouteValueDictionary values,
		RouteDirection routeDirection
	)
		=> values.TryGetValue(routeKey, out var value)
		   && value is string { Length: > 0 } s
		   && Base64Url.IsValid(s);
}