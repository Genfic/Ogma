using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Ogma3.Services.SiteConfig
{
    public interface ISiteConfig
    {
        /// <summary>
        /// Dictionary that holds the config
        /// </summary>
        public Dictionary<string, string> Config { get; set; }

        /// <summary>
        /// Set value in the `Config` dictionary
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <param name="value">Value to set the key to</param>
        /// <typeparam name="T">Type of the value</typeparam>
        public void SetValue<T>(string key, T value);

        /// <summary>
        /// Removes the key from the `Config` dictionary
        /// </summary>
        /// <param name="key">Key to remove</param>
        public void DeleteKey(string key);

        /// <summary>
        /// Get a raw value of the given key in the `Config` dictionary
        /// </summary>
        /// <param name="key">Key of the value to get</param>
        /// <returns>The value of the key as string</returns>
        public string GetRawValue(string key);

        /// <summary>
        /// Get a value of the given key as the desired type `T` from the `Config` dictionary
        /// </summary>
        /// <param name="key">Key of the value to get</param>
        /// <typeparam name="T">Type of the desired value</typeparam>
        /// <returns>The value of the key as type `T`</returns>
        [return: MaybeNull]
        public T GetValue<T>(string key);

        /// <summary>
        /// Persist the current config on some persistent storage asynchronously
        /// </summary>
        public Task PersistAsync();

        /// <summary>
        /// Persist the current config on some persistent storage synchronously
        /// </summary>
        public void Persist();
    }
}