namespace Ogma3.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowBannedUsersAttribute : Attribute;