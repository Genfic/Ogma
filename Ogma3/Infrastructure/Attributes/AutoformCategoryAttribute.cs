namespace Ogma3.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AutoformCategoryAttribute(string name) : Attribute
{
	public string Name { get; } = name;
}