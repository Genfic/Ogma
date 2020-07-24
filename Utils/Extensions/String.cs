using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utils.Extensions
{
    public static class String
    {
        /// <summary>
        /// Replace non-alphanumeric characters with underscores, and double underscores with single ones.
        /// </summary>
        /// <param name="input">String to friendlify</param>
        /// <returns>Friendlified string</returns>
        public static string Friendlify(this string input)
        {
            var str = new Regex("[^a-zA-Z0-9]").Replace(input, "-");
            str = new Regex("-+").Replace(str, "-");

            return str.ToLower().Trim('-');
        }
        
        /// <summary>
        /// Replaces elements of the `template` according to the supplied `pattern`
        /// </summary>
        /// <param name="template">Template to replace values in</param>
        /// <param name="pattern">Dictionary in which keys are values to be replaced and values are values to replace them with</param>
        /// <returns>Resulting string</returns>
        public static string ReplaceWithPattern(this string template, Dictionary<string, string> pattern)
        {
            foreach (var (key, value) in pattern)
            {
                template = template.Replace(key, value);
            }
            return template;
        }
    }
}