using System.Text;

namespace Utils.Extensions;

public static class UriExtensions
{
	public static Uri AppendSegments(this Uri uri, params string[] segments)
	{
		var sb = new StringBuilder();
		
		sb.Append(uri.ToString().TrimEnd('/'));
		foreach (var segment in segments)
		{
			sb.Append('/');
			sb.Append(segment);
		}

		return new Uri(sb.ToString());
	}
}