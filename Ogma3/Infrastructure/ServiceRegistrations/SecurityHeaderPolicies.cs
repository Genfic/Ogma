namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class SecurityHeaderPolicies
{
	public const string Lax = nameof(Lax);

	public static IServiceCollection AddCustomSecurityHeaderPolicies(this IServiceCollection services)
	{
		return services.AddSecurityHeaderPolicies(builder => {

			builder.SetDefaultPolicy(policy => {
				policy.AddFrameOptionsDeny();
				policy.AddXssProtectionBlock();
				policy.AddContentTypeOptionsNoSniff();
				policy.AddReferrerPolicyStrictOriginWhenCrossOrigin();
				policy.AddContentSecurityPolicy(csp => {
					csp.AddDefaultSrc()
						.Self()
						.WithNonce();
					csp.AddImgSrc()
						.Self()
						.From("https://ipfs.io") // NOTE: testing only
						.From("https://picsum.photos") // NOTE: testing only
						.From("https://*.picsum.photos") // NOTE: testing only
						.From("https://genfic.net")
						.From("https://*.genfic.net");
					csp.AddStyleSrc()
						.Self()
						.UnsafeInline();
					csp.AddObjectSrc()
						.None();
					csp.AddFormAction()
						.Self();
					csp.AddFrameAncestors()
						.None();
					csp.AddScriptSrc()
						.Self()
						.WithNonce();
				});
			});

			builder.AddPolicy(Lax, policy =>
				policy.AddContentSecurityPolicy(csp => {
					csp.AddScriptSrc()
						.Self()
						.UnsafeInline();
				})
			);
		});
	}
}