using System.Threading.Tasks;

namespace Ogma3.Services.TurnstileService;

public interface ITurnstileService
{
	public Task<TurnstileResult> Verify(string token, string ip);
}