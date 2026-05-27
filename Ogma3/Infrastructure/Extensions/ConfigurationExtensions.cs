namespace Ogma3.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
	public static T Require<T>(this IConfigurationManager manager, string key)
	{
		return manager.GetValue<T>(key) ?? throw new InvalidOperationException($"Configuration key {key} is not set");
	}
}