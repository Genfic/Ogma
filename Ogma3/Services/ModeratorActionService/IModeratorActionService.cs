using Ogma3.Data.Infractions;

namespace Ogma3.Services.ModeratorActionService;

public interface IModeratorActionService
{
    void LogInfractionCreated(long userId, long infractionId, string reason, InfractionType type);
    void LogInfractionLifted(long userId, long infractionId, DateTimeOffset? activeUntil, DateTimeOffset? removedAt, InfractionType type);
    void LogUserRolesChanged(long targetUserId, string targetUserName, long[] oldRoles, long[] newRoles);
    void LogContentBlocked(string contentType, string title, long contentId);
    void LogContentUnblocked(string contentType, string title, long contentId);
    void LogThreadLocked(string contentType, long contentId, long threadId);
    void LogThreadUnlocked(string contentType, long contentId, long threadId);
}