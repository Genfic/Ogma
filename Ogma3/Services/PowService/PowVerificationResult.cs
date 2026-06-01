using NetEscapades.EnumGenerators;

namespace Ogma3.Services.PowService;

[EnumExtensions]
public enum PowVerificationResult
{
	Ok,
	NotFound,
	Invalid,
	TooFast,
}