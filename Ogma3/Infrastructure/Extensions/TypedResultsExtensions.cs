using Ogma3.Infrastructure.IResults;

namespace Ogma3.Infrastructure.Extensions;

public static class TypedResultsExtensions
{
	extension(TypedResults)
	{
		public static NotModifiedResult NotModified() => new();
		public static UnauthorizedResult ProperUnauthorized() => new();
	}
}