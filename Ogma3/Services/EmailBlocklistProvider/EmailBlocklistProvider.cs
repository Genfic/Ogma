using System.Collections.Frozen;

namespace Ogma3.Services.EmailBlocklistProvider;

public sealed class EmailBlocklistProvider : IEmailBlocklistProvider
{
	private readonly FrozenSet<string> _domains;
	private EmailBlocklistProvider(string[] domains)
	{
		_domains = new HashSet<string>(domains)
			.Where(l => !string.IsNullOrWhiteSpace(l))
			.Select(l => l.Trim())
			.ToFrozenSet();
	}

	public static async Task<EmailBlocklistProvider> CreateAsync()
	{
		var lines = await File.ReadAllLinesAsync("disposable_email_blocklist.conf");
		return new EmailBlocklistProvider(lines);
	}

	public bool IsDisposable(string email) => _domains.Contains(email.Split('@').Last());
}