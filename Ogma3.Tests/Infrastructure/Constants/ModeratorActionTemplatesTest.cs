using Ogma3.Data.Infractions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Tests.Infrastructure.Constants;

public sealed class ModeratorActionTemplatesTest
{
	private const string Mod = "Moderator";
	private const long UserId = 7;
	private const string UserName = "Ursula";

	private static OgmaUser User() => new() { Id = UserId, UserName = UserName };

	[Test]
	public async Task TestUserMute()
	{
		var msg = ModeratorActionTemplates.UserMute(User(), Mod, DateTimeOffset.UtcNow.AddDays(30));
		await Assert.That(msg).Contains(UserName);
		await Assert.That(msg).Contains($"(id: {UserId})");
		await Assert.That(msg).Contains(Mod);
	}

	[Test]
	public async Task TestUserUnmute()
	{
		var msg = ModeratorActionTemplates.UserUnmute(User(), Mod, DateTimeOffset.UtcNow.AddMinutes(10));
		await Assert.That(msg).Contains("has been unmuted");
		await Assert.That(msg).Contains(Mod);
	}

	[Test]
	public async Task TestUserRoleRemoved()
	{
		var msg = ModeratorActionTemplates.UserRoleRemoved(User(), Mod, "Editors");
		await Assert.That(msg).Contains("role **Editors**");
	}

	[Test]
	public async Task TestUserRoleAdded()
	{
		var msg = ModeratorActionTemplates.UserRoleAdded(User(), Mod, "Editors");
		await Assert.That(msg).Contains("granted a **Editors** role");
	}

	[Test]
	public async Task TestUserRolesChanged()
	{
		var msg = ModeratorActionTemplates.UserRolesChanged(UserName, UserId, Mod, [1, 2], [2, 3]);
		await Assert.That(msg).Contains("[1, 2]");
		await Assert.That(msg).Contains("[2, 3]");
	}

	[Test]
	public async Task TestContentBlocked()
	{
		var msg = ModeratorActionTemplates.ContentBlocked("Story", "My Title", 42, Mod);
		await Assert.That(msg).Contains("***\"My Title\"***");
		await Assert.That(msg).Contains("has been blocked");
	}

	[Test]
	public async Task TestContentUnblocked()
	{
		var msg = ModeratorActionTemplates.ContentUnblocked("Story", "My Title", 42, Mod);
		await Assert.That(msg).Contains("***\"My Title\"***");
		await Assert.That(msg).Contains("has been unblocked");
	}

	[Test]
	public async Task TestThreadLocked()
	{
		var msg = ModeratorActionTemplates.ThreadLocked("Story", 1, 2, Mod);
		await Assert.That(msg).Contains("was locked");
		await Assert.That(msg).Contains("(id: 1)");
		await Assert.That(msg).Contains("**2**");
	}

	[Test]
	public async Task TestThreadUnlocked()
	{
		var msg = ModeratorActionTemplates.ThreadUnlocked("Story", 1, 2, Mod);
		await Assert.That(msg).Contains("was unlocked");
		await Assert.That(msg).Contains("(id: 1)");
		await Assert.That(msg).Contains("**2**");
	}

	[Test]
	public async Task TestForumThreadDeleted()
	{
		var msg = ModeratorActionTemplates.ForumThreadDeleted("Club Name", 99, Mod);
		await Assert.That(msg).Contains("Club Name");
		await Assert.That(msg).Contains("**99**");
	}

	[Test]
	public async Task TestUserBan()
	{
		var msg = ModeratorActionTemplates.UserBan("Troll", Mod, "Being a troll");
		await Assert.That(msg).Contains("Troll");
		await Assert.That(msg).Contains("Being a troll");
	}

	[Test]
	public async Task TestUserUnban()
	{
		var msg = ModeratorActionTemplates.UserUnban("Troll", Mod);
		await Assert.That(msg).Contains("was unbanned");
	}

	[Test]
	public async Task TestInfractionCreate()
	{
		var msg = ModeratorActionTemplates.Infractions.Create(UserId, Mod, 5, "Reason", InfractionType.Note);
		await Assert.That(msg).Contains("**Note**");
		await Assert.That(msg).Contains("Reason");
	}

	[Test]
	public async Task TestInfractionLift()
	{
		var msg = ModeratorActionTemplates.Infractions.Lift(UserId, Mod, 5, InfractionType.Warning);
		await Assert.That(msg).Contains("**Warning**");
	}
}