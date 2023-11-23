using System.Text.Json;

namespace Ogma3.Infrastructure.Json;

public static class SerializerOptions
{
	public static JsonSerializerOptions Indented => new()
	{
		WriteIndented = true,
	};
}