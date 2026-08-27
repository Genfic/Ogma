using System.Text.Json.Serialization;
using Aspire.Hosting.Pipelines;

namespace Ogma3.AppHost.Helpers;

#pragma warning disable ASPIREPIPELINES001

public static class WebhookSender
{
	public const string WebhookNotifyName = "notify-deploy-webhook";

	public static void AddDiscordWebhookNotify(this IDistributedApplicationBuilder builder)
	{
		builder.Pipeline.AddStep(WebhookNotifyName, static async ctx => {
			var cfg = ctx.Services.RequireConfigSection<DiscordWebhook>("DiscordWebhook");
			var git = ctx.Services.GetGitState();

			using var client = new HttpClient();

			var body = new DiscordWebhookMessage([
				new ("The website has been updated!", 16737280, $$"""Current build: `{{{git.Hash}}}`"""),
			]);

			var res = await client.PostAsJsonAsync(
				cfg.Url,
				body,
				DiscordWebhookJsonContext.Default.DiscordWebhookMessage,
				ctx.CancellationToken
			);
			res.EnsureSuccessStatusCode();

			ctx.Summary.Add("🔔 Discord Webhook", "Sent");

		}, dependsOn: WellKnownPipelineSteps.Deploy);
	}

	private sealed record DiscordWebhook(string Url);
}

public sealed record DiscordWebhookMessage(DiscordWebhookEmbed[] Embeds);
public sealed record DiscordWebhookEmbed(string Title, long Color, string Description);


[JsonSerializable(typeof(DiscordWebhookMessage))]
public partial class DiscordWebhookJsonContext : JsonSerializerContext;
