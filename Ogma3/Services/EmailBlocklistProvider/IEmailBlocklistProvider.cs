namespace Ogma3.Services.EmailBlocklistProvider;

public interface IEmailBlocklistProvider
{
	bool IsDisposable(string email);
}