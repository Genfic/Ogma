using System;
using System.Text.Json;

namespace Ogma3.Services;

public static class Jog
{
    public static void Log(object obj)
    {
        Console.WriteLine(JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}