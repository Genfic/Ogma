using System;
using System.Text.Json;
using Ogma3.Infrastructure.Json;

namespace Ogma3.Services;

public static class Jog
{
	public static void Log(object obj)
	{
		Console.WriteLine(JsonSerializer.Serialize(obj, SerializerOptions.Indented));
	}
}