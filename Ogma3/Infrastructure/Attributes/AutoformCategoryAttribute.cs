using System;

namespace Ogma3.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoformCategoryAttribute : Attribute
    {
        private readonly string _name;
        public string Name => _name;

        public AutoformCategoryAttribute(string name)
        {
            _name = name;
        }
    }
}