namespace Ogma3.Services.SpeedTrapService;

public interface ISpeedTrapService
{
	string GenerateToken();
	bool IsHumanSpeed(string token, int minimumSeconds);
}