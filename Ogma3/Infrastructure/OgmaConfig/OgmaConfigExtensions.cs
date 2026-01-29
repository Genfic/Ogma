namespace Ogma3.Infrastructure.OgmaConfig;

public static class OgmaConfigExtensions
{
	public static async Task AddOgmaConfigAsync(this IServiceCollection services, string filePath)
	{
		var persistence = await OgmaConfigPersistence.InitAsync(filePath);
		services.AddSingleton(persistence.Config);
		services.AddSingleton(persistence);
	}
}