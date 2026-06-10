using System.Security.Cryptography;

namespace Ogma3.AppHost.Helpers;

public static class Password
{
	private const string Alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-=!@#$%^&*()_+[]{}|;:,.<>?";

	public static string Generate(int length, bool log)
	{
		var chars = RandomNumberGenerator.GetItems(Alpha.AsSpan(), length);
		var pwd = new string(chars);

		if (log)
		{
			Console.Out.WriteLine($"Generated dashboard password: {pwd}");
		}

		return pwd;
	}
}