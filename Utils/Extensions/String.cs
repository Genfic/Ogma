using System;
using System.Collections.Generic;
using System.Linq;
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
        
        /// <summary>
        /// Removes all leading whitespace
        /// </summary>
        /// <param name="input">String to modify</param>
        /// <returns>String without leading whitespace</returns>
        public static string RemoveLeadingWhiteSpace(this string input)
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Select(s => s.TrimStart(' ', '\t')));
        }

        public static IEnumerable<Header> GetMarkdownHeaders(this string input)
        {
            var headers = new List<Header>();
            
            var lines = input.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                if (!line.StartsWith('#')) continue;
                
                var head = new Header();
                foreach (var c in line)
                {
                    if (c == '#')
                    {
                        head.Level++;
                    }
                    else
                    {
                        break;
                    }
                }
                head.Body = line.TrimStart('#').Trim();

                var latest = headers
                    .Where(h => h.Body == head.Body)
                    .OrderByDescending(h => h.Occurrence)
                    .FirstOrDefault();

                if (latest != null)
                {
                    head.Occurrence =  (byte) (latest.Occurrence + 1);
                }

                headers.Add(head);
            }

            return headers;
        }
        
        public class Header
        {
            public byte Level { get; set; }
            public byte Occurrence { get; set; }
            public string Body { get; set; }
        }
    }
}