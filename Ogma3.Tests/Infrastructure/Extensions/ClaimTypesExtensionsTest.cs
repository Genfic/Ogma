using System.Security.Claims;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Tests.Infrastructure.Extensions;

public sealed class ClaimTypesExtensionsTest
{
	[Test]
	public async Task TestAllClaimTypeConstants()
	{
		await Assert.That(ClaimTypes.Avatar).IsEqualTo("x:Avatar");
		await Assert.That(ClaimTypes.Title).IsEqualTo("x:Title");
		await Assert.That(ClaimTypes.IsStaff).IsEqualTo("x:IsStaff");
		await Assert.That(ClaimTypes.Timezone).IsEqualTo("x:Timezone");
		await Assert.That(ClaimTypes.ImpersonatingUserId).IsEqualTo("x:ImpersonatingUserId");
	}
}