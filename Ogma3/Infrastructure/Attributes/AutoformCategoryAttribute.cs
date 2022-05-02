using System;

namespace Ogma3.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AutoformCategoryAttribute : Attribute
{
	public string Name { get; }

	public AutoformCategoryAttribute(string name)
	{
		Name = name;
	}
}