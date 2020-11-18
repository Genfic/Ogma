using System;
using Humanizer;
using Humanizer.Localisation;
using Ogma3.Data.Models;

namespace Ogma3.Data
{
    public static class ModeratorActionTemplates
    {
        public static string UserBan(OgmaUser user, string modName, DateTime until) 
            => $"User **{user.UserName}** (id: {user.Id}) has been banned until {until} by **{modName}**.";
        
        // BUG: shit's throwing Humanizer exceptions
        public static string UserUnban(OgmaUser user, string modName, DateTime until) 
            => $"User **{user.UserName}** (id: {user.Id}) has been unbanned {(until - DateTime.Now).Humanize(3, minUnit: TimeUnit.Minute)} early by **{modName}**.";
        
        public static string UserMute(OgmaUser user, string modName, DateTime until) 
            => $"User **{user.UserName}** (id: {user.Id}) has been muted until {until} by **{modName}**.";
        
        public static string UserUnmute(OgmaUser user, string modName, DateTime until) 
            => $"User **{user.UserName}** (id: {user.Id}) has been unmuted {(until - DateTime.Now).Humanize(3, minUnit: TimeUnit.Minute)} early by **{modName}**.";
    }
}