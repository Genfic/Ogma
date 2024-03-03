using System;

namespace Ogma3.Infrastructure.Exceptions;

public class UnexpectedEnumValueException<TEnum> : Exception where TEnum : Enum
{
	public UnexpectedEnumValueException(TEnum value) : base($"Value {value} of enum {typeof(TEnum).Name} is not supported")
	{ }
	
	public UnexpectedEnumValueException(TEnum value, string paramName) : base($"Value {value} of enum {typeof(TEnum).Name} stored in {paramName} is not supported")
	{ }
}