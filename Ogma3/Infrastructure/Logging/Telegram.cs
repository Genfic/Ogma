using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace Ogma3.Infrastructure.Logging;

public static class Telegram
{
	public static async Task<TelegramToken> GetCredentials()
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

		if (split is [string token, string id]) return new(token, id);

		Log.Fatal("Expected two elements of the token, {Count} found", split.Length);
		throw new ArgumentException($"Expected two elements of the Telegram token, {split.Length} found");
	}

	public record TelegramToken(string Token, string Id);
}