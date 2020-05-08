using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Utils
{
    public static class Lorem
    {
        public static string Picsum(int x, int? y = null)
        {
            return y == null ? $"//picsum.photos/{x}" : $"//picsum.photos/{x}/{y}";
        }

        public static string Ipsum(int paragraphs, IpsumOptions options)
        {
            var api = $"https://loripsum.net/api/{paragraphs}/";

            if (options != null)
            {
                var parts = new List<string>();
                if (options.Length != null) parts.Add(options.Length.ToString().ToLower());
                if (options.Decorate) parts.Add("decorate");
                if (options.Link) parts.Add("link");
                if (options.Ulist) parts.Add("ul");
                if (options.Olist) parts.Add("ol");
                if (options.Dlist) parts.Add("dl");
                if (options.Blockquotes) parts.Add("bq");
                if (options.Codeblocks) parts.Add("code");
                if (options.Headers) parts.Add("headers");
                if (options.Allcaps) parts.Add("allcaps");
                if (options.Prude) parts.Add("prude");
                if (options.Plaintext) parts.Add("plaintext");
                api += string.Join("/", parts);
            }

            using var client = new WebClient();
            return client.DownloadString(api);
        }

        public static string Gravatar(string email, int size = 200)
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()))
                .ToList()
                .Select(x => x.ToString("x2"));
            var hash = string.Join("", data);
            return $"https://www.gravatar.com/avatar/{hash}?s={size}";
        }
    }

    public class IpsumOptions
    {
        public IpsumLength? Length { get; set; }
        public bool Decorate { get; set; }
        public bool Link { get; set; }
        public bool Ulist { get; set; }
        public bool Olist { get; set; }
        public bool Dlist { get; set; }
        public bool Blockquotes { get; set; }
        public bool Codeblocks { get; set; }
        public bool Headers { get; set; }
        public bool Allcaps { get; set; }
        public bool Prude { get; set; }
        public bool Plaintext { get; set; }
    }

    public enum IpsumLength
    {
        Short,
        Medium,
        Long,
        Verylong
    }
}