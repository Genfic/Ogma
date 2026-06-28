using NetEscapades.EnumGenerators;

namespace Ogma3.Data.Subscriptions;

[EnumExtensions]
[Flags]
public enum Entitlement : ushort
{
	None = 0,
	AdFree = 1 << 0,
	AnimatedAvatar = 1 << 1,
	DraftsLastLonger = 1 << 2,
	DraftsLastForever = 1 << 3,
	BronzeAccolade = 1 << 4,
	SilverAccolade = 1 << 5,
	GoldAccolade = 1 << 6,
}