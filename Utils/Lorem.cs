#nullable enable

using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Lorem
    {
        public static string Picsum(int x, int? y = null) => y is null ? $"//picsum.photos/{x}" : $"//picsum.photos/{x}/{y}";

        public static async Task<string> Ipsum(int paragraphs, IpsumOptions? options)
        {
            var sb = new StringBuilder();
            sb.Append("https://loripsum.net/api/");
            sb.Append(paragraphs);

            if (options is not null)
            {
                if (options.Length is not null) sb.Append($"/{options.Length}".ToLower());
                if (options.Decorate) sb.Append("/decorate");
                if (options.Link) sb.Append("/link");
                if (options.Ulist) sb.Append("/ul");
                if (options.Olist) sb.Append("/ol");
                if (options.Dlist) sb.Append("/dl");
                if (options.Blockquotes) sb.Append("/bq");
                if (options.Codeblocks) sb.Append("/code");
                if (options.Headers) sb.Append("/headers");
                if (options.Allcaps) sb.Append("/allcaps");
                if (options.Prude) sb.Append("/prude");
                if (options.Plaintext) sb.Append("/plaintext");
            }

            using var client = new HttpClient();
            return await client.GetStringAsync(sb.ToString());
        }

        public static string Gravatar(string email, int size = 200)
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()))
                .Select(x => x.ToString("x2"));
            var hash = string.Join("", data);
            return $"https://www.gravatar.com/avatar/{hash}?s={size}";
        }
    }

    public record IpsumOptions
    {
        public IpsumLength? Length { get; init; }
        public bool Decorate { get; init; }
        public bool Link { get; init; }
        public bool Ulist { get; init; }
        public bool Olist { get; init; }
        public bool Dlist { get; init; }
        public bool Blockquotes { get; init; }
        public bool Codeblocks { get; init; }
        public bool Headers { get; init; }
        public bool Allcaps { get; init; }
        public bool Prude { get; init; }
        public bool Plaintext { get; init; }
    }

    public enum IpsumLength
    {
        Short,
        Medium,
        Long,
        Verylong
    }
}