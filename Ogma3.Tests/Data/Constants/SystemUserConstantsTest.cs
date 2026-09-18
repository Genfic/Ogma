using Ogma3.Data.Constants;

namespace Ogma3.Tests.Data.Constants;

public sealed class SystemUserConstantsTest
{
	[Test]
	public async Task TestDeletedUser()
	{
		var user = SystemUserConstants.Deleted;
		await Assert.That(user.Id).IsEqualTo(-1);
		await Assert.That(user.Name).IsEqualTo("Deleted User");
		await Assert.That(user.NormalizedName).IsEqualTo("DELETED USER");
		await Assert.That(user.Avatar).IsEqualTo("/img/placeholders/deleted-user.png");
	}

	[Test]
	public async Task TestAnonymousUser()
	{
		var user = SystemUserConstants.Anonymous;
		await Assert.That(user.Id).IsEqualTo(-2);
		await Assert.That(user.Name).IsEqualTo("Anonymous User");
		await Assert.That(user.NormalizedName).IsEqualTo("ANONYMOUS USER");
		await Assert.That(user.Avatar).IsEqualTo("/img/placeholders/anonymous-user.png");
	}

	[Test]
	public async Task TestRecordEquality()
	{
		await Assert.That(SystemUserConstants.Deleted).IsEqualTo(new SystemUserConstants.SystemUser(-1, "Deleted User"));
		await Assert.That(SystemUserConstants.Deleted).IsNotEqualTo(SystemUserConstants.Anonymous);
	}
}