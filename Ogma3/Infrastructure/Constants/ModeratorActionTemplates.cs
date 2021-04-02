using System;
using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using Ogma3.Data.Users;

namespace Ogma3.Data
{
    public static class ModeratorActionTemplates
    {
        private static string HumanizeTimespan(this TimeSpan ts) 
            => ts.Humanize(3, minUnit: TimeUnit.Minute, culture: CultureInfo.InvariantCulture);
        
        // Ban templates
        public static string UserBan(OgmaUser user, string modName, DateTime until) 
            => $"User **{user.UserName}** (id: {user.Id}) has been banned until {until} by **{modName}**.";
        public static string UserUnban(OgmaUser user, string modName, DateTime until) 
            => $"User **{user.UserName}** (id: {user.Id}) has been unbanned {(until - DateTime.Now).HumanizeTimespan()} early by **{modName}**.";
        
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
    }
}