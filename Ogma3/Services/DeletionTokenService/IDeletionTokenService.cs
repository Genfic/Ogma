namespace Ogma3.Services.DeletionTokenService;

public interface IDeletionTokenService
{
	string GenerateToken(long contentId, DateTimeOffset scheduledFor, string contentType);
	bool TryParseToken(string token, out long contentId, out DateTimeOffset scheduledFor, out string contentType);
}