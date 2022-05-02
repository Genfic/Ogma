#region

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Utils;

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
}

public record IpsumOptions(
	IpsumLength? Length,
	bool Decorate,
	bool Link,
	bool Ulist,
	bool Olist,
	bool Dlist,
	bool Blockquotes,
	bool Codeblocks,
	bool Headers,
	bool Allcaps,
	bool Prude,
	bool Plaintext);

public enum IpsumLength
{
	Short,
	Medium,
	Long,
	Verylong
}