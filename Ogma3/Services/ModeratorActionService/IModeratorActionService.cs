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

    // User management
    void LogUserCreated(long userId, string userName);
    void LogUserImpersonated(long targetUserId, string targetUserName);

    // News
    void LogNewsCreated(long newsId, string title);
    void LogNewsUpdated(long newsId, string title);

    // Documents
    void LogDocumentCreated(string slug, string title);
    void LogDocumentUpdated(string slug, string title, uint version);

    // Tags
    void LogTagCreated(long tagId, string name, string namespaceName);
    void LogTagUpdated(long tagId, string name, string namespaceName);
    void LogTagDeleted(long tagId, string name, string namespaceName);

    // Quotes
    void LogQuoteCreated(long quoteId, string author);
    void LogQuoteUpdated(long quoteId, string author);
    void LogQuoteDeleted(long quoteId, string author);

    // Ratings
    void LogRatingCreated(long ratingId, string name);
    void LogRatingUpdated(long ratingId, string name);
    void LogRatingDeleted(long ratingId, string name);

    // FAQs
    void LogFaqCreated(long faqId, string question);
    void LogFaqUpdated(long faqId, string question);
    void LogFaqDeleted(long faqId, string question);

    // Roles
    void LogRoleCreated(long roleId, string name, bool isStaff);
    void LogRoleUpdated(long roleId, string name, bool isStaff);
    void LogRoleDeleted(long roleId, string name);
    void LogInviteCodeCreated();
}