namespace Ogma3.Infrastructure.Exceptions;

public sealed class NotAuthenticatedException : Exception
{
	public NotAuthenticatedException() {}
	public NotAuthenticatedException(string message) : base(message) {}
}