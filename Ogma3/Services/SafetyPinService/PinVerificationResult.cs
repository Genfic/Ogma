namespace Ogma3.Services.SafetyPinService;

public enum PinVerificationResult
{
	Valid,
	Invalid,
	LockedOut,
	NoPin,
	NotFound,
}