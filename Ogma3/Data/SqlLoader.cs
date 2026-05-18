using System.Reflection;

namespace Ogma3.Data;

public static class SqlLoader
{
	public static string Load(string fileName)
	{
		var assembly = Assembly.GetExecutingAssembly();

		using var stream = assembly.GetManifestResourceStream(fileName) ?? throw new InvalidOperationException($"Could not find embedded resource {fileName}");
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}

	public static string LoadSql(this EmbeddedResourceQueries resource)
	{
		return resource.GetReader().ReadToEnd();
	}
}