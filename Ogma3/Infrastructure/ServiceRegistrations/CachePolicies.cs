namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class CachePolicies
{
	public const string Rss = nameof(Rss);
	public static IServiceCollection AddCachePolicies(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddOutputCache(options => {
			options.AddPolicy(Rss, builder => builder.Expire(TimeSpan.FromMinutes(45)));
		});
		return serviceCollection;
	}
}