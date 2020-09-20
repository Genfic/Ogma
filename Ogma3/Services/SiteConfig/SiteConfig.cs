#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ogma3.Services.SiteConfig
{
    public class SiteConfig : ISiteConfig
    {
        public Dictionary<string, string> Config { get; set; }
        private readonly JsonSerializerOptions _serializerOptions;

        public SiteConfig(Dictionary<string, string> initial)
        {
            Config = initial;
            _serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public void SetValue<T>(string key, T value)
        {
            if (value == null) return;
            
            var val = value.ToString();
            if (val != null)
            {
                Config[key] = val;
            }
        }

        public void DeleteKey(string key)
        {
            Config.Remove(key);
        }

        public string? GetRawValue(string key)
        {
            return Config.TryGetValue(key, out var val) ? val : null;
        }

        [return: MaybeNull]
        public T GetValue<T>(string key)
        {
            if (Config.TryGetValue(key, out var val))
            {
                return (T) Convert.ChangeType(val, typeof(T));
            }
            return default;
        }

        public void Persist()
        {
            using var sw = new StreamWriter("config.json");
            var json = JsonSerializer.Serialize(Config, _serializerOptions);
            sw.Write(json);
        }

        public async Task PersistAsync()
        {
            await using var sw = new StreamWriter("config.json");
            var json = JsonSerializer.Serialize(Config, _serializerOptions);
            await sw.WriteAsync(json);
        }
    }
}