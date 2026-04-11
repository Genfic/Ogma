using System.Collections.Frozen;

namespace Ogma3.Services.EmailBlocklistProvider;

[RegisterSingleton<IEmailBlocklistProvider>]
public sealed class EmailBlocklistProvider : IEmailBlocklistProvider
{
	private readonly FrozenSet<string> _domains;
	public EmailBlocklistProvider()
	{
		var lines = File.ReadAllLines("disposable_email_blocklist.txt");
		_domains = new HashSet<string>(lines)
			.Select(l => l.Trim())
			.Where(l => l.Length > 0)
			.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	}

	public bool IsDisposable(string email)
	{
		var domain = email[(email.LastIndexOf('@') + 1)..];
		return _domains.Contains(domain);
	}
}