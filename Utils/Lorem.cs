using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Utils;

public static class Lorem
{
	public static string Picsum(int x, int? y = null) => y is null ? $"//picsum.photos/{x}" : $"//picsum.photos/{x}/{y}";

	public static async Task<string> Ipsum(int paragraphs, IpsumOptions? options)
	{
		var sb = new StringBuilder();
		sb.Append("https://loripsum.net/api/");
		sb.Append(paragraphs);

		if (options is {} o)
		{
			if (o.Length is not null) sb.Append($"/{o.Length}".ToLower());
			if (o.Decorate) sb.Append("/decorate");
			if (o.Link) sb.Append("/link");
			if (o.Ulist) sb.Append("/ul");
			if (o.Olist) sb.Append("/ol");
			if (o.Dlist) sb.Append("/dl");
			if (o.Blockquotes) sb.Append("/bq");
			if (o.Codeblocks) sb.Append("/code");
			if (o.Headers) sb.Append("/headers");
			if (o.Allcaps) sb.Append("/allcaps");
			if (o.Prude) sb.Append("/prude");
			if (o.Plaintext) sb.Append("/plaintext");
		}

		using var client = new HttpClient();
		return await client.GetStringAsync(sb.ToString());
	}
}

[UsedImplicitly]
public readonly record struct IpsumOptions(
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
	Verylong,
}