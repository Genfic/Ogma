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
					csp.AddObjectSrc().None();
					csp.AddFormAction().Self();
					csp.AddFrameAncestors().None();
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