using System;

namespace Ogma3.Codegen.PostgresEnumRegistrationGenerator
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class PostgresEnumAttribute : Attribute
    {
        private readonly string _name;

        public PostgresEnumAttribute(string name = "")
        {
            _name = name;
        }
    }
}