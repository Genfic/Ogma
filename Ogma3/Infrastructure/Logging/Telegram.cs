using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace Ogma3.Infrastructure.Logging;

public static class Telegram
{
	public static async Task<(string Token, string Id)> GetCredentials()
	{
		var telegramToken = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

		if (telegramToken is not { Length: > 0 })
		{
			try
			{
				using var sr = new StreamReader("./logger-tokens.txt");
				telegramToken = await sr.ReadToEndAsync();
			}
			catch (Exception e)
			{
				Log.Fatal(e, "Fatal error occurred when trying to read logger tokens");
				throw;
			}
		}

		var split = telegramToken.Split('|');

		if (split.Length >= 2) return (Token: split[0], Id: split[1]);

		Log.Fatal("Expected two elements of the token, {Count} found", split.Length);
		throw new ArgumentException($"Expected two elements of the Telegram token, {split.Length} found");
	}
}