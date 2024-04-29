using System;
using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using Ogma3.Data.Infractions;
using Ogma3.Data.Users;

namespace Ogma3.Infrastructure.Constants;

public static class ModeratorActionTemplates
{
	private static string HumanizeTimespan(this TimeSpan ts)
		=> ts.Humanize(3, minUnit: TimeUnit.Minute, culture: CultureInfo.InvariantCulture);

	// Mute templates
	public static string UserMute(OgmaUser user, string modName, DateTime until)
		=> $"User **{user.UserName}** (id: {user.Id}) has been muted until {until} by **{modName}**.";

	public static string UserUnmute(OgmaUser user, string modName, DateTime until)
		=> $"User **{user.UserName}** (id: {user.Id}) has been unmuted {(until - DateTime.Now).HumanizeTimespan()} early by **{modName}**.";

	// Role templates
	public static string UserRoleRemoved(OgmaUser user, string modName, string roleName)
		=> $"User **{user.UserName}** (id: {user.Id}) had their role **{roleName}** removed by **{modName}**.";

	public static string UserRoleAdded(OgmaUser user, string modName, string roleName)
		=> $"User **{user.UserName}** (id: {user.Id}) has been granted a **{roleName}** role by **{modName}**.";

	// Content templates
	public static string ContentBlocked(string type, string title, long id, string modName)
		=> $"{type.Humanize()} ***\"{title}\"*** (id: {id}) has been blocked by **{modName}**";

	public static string ContentUnblocked(string type, string title, long id, string modName)
		=> $"{type.Humanize()} ***\"{title}\"*** (id: {id}) has been unblocked by **{modName}**";

	// Thread lock templates
	public static string ThreadLocked(string type, long id, long threadId, string modName)
		=> $"Comment thread for **{type}** (id: {id}) with the ID **{threadId}** was locked by **{modName}**";

	public static string ThreadUnlocked(string type, long id, long threadId, string modName)
		=> $"Comment thread for **{type}** (id: {id}) with the ID **{threadId}** was unlocked by **{modName}**";

	// Forum thread delete templates
	public static string ForumThreadDeleted(string title, long threadId, string modName)
		=> $"Forum thread in club **{title}** with the ID **{threadId}** was deleted by **{modName}**";

	// Ban templates
	public static string UserBan(string bannedName, string modName, string reason)
		=> $"User **{bannedName}** was banned by **{modName}** for the following reason:\n*{reason}*";

	public static string UserUnban(string bannedName, string modName)
		=> $"User **{bannedName}** was unbanned by **{modName}**";

	public static class Infractions
	{
		public static string Create(long userId, string modName, long infractionId, string reason, InfractionType type)
			=> $"User **{userId}** was given a **{type.ToStringFast()}** infraction ({infractionId}) by **{modName}** for the following reason:\n*{reason}*";

		public static string Lift(long userId, string modName, long infractionId, InfractionType type)
			=> $"User **{userId}** had their **{type.ToStringFast()}** infraction ({infractionId}) lifted by **{modName}**.";

	}
}