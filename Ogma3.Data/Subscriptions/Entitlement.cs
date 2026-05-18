using NetEscapades.EnumGenerators;

namespace Ogma3.Data.Subscriptions;

[EnumExtensions]
public enum Entitlement : ushort
{
	AdFree,
	AnimatedAvatar,
	Trim,
	DraftsLastLonger,
	DraftsLastForever,
}