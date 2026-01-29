using System.Collections.Frozen;

namespace Ogma3.Services.EmailBlocklistProvider;

public sealed class EmailBlocklistProvider : IEmailBlocklistProvider
{
	private readonly FrozenSet<string> _domains;
	private EmailBlocklistProvider(string[] domains)
	{
		_domains = new HashSet<string>(domains)
			.Select(l => l.Trim())
			.Where(l => l.Length > 0)
			.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	}

	public static async Task<EmailBlocklistProvider> CreateAsync()
	{
		var lines = await File.ReadAllLinesAsync("disposable_email_blocklist.txt");
		return new EmailBlocklistProvider(lines);
	}

	public bool IsDisposable(string email)
	{
		var domain = email[(email.LastIndexOf('@') + 1)..];
		return _domains.Contains(domain);
	}
}