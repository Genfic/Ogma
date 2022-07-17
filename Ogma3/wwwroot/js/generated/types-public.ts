export interface ErrorControllerResult {
    code: number | null;
    reason: string | null;
}

export interface GetVotesResult {
    count: number;
    didVote: boolean;
}

export interface CreateVoteResult {
    didVote: boolean;
    count: number | null;
}

export interface CreateVoteCommand {
    storyId: number;
}

export interface DeleteVoteResult {
    didVote: boolean;
    count: number | null;
}

export interface DeleteVoteCommand {
    storyId: number;
}

export interface ProblemDetails {
    type: string | null;
    title: string | null;
    status: number | null;
    detail: string | null;
    instance: string | null;
}

export interface BlockUserCommand {
    name: string | null;
}

export interface UnblockUserCommand {
    name: string | null;
}

export interface FollowUserCommand {
    name: string | null;
}

export interface UnfollowUserCommand {
    name: string | null;
}

export interface UpdateRolesCommand {
    userId: number;
    roles: object[];
}

export interface TagDto {
    id: number;
    name: string | null;
    slug: string | null;
    description: string | null;
    namespaceColor: string | null;
    namespaceId: number | null;
}

export type ETagNamespace = "ContentWarning" | "Genre" | "Franchise";

export interface UpdateTagCommand {
    id: number;
    name: string | null;
    description: string | null;
}

export interface CreateTagCommand {
    name: string;
    description: string | null;
}

export interface SubscribeCommentsThreadCommand {
    threadId: number;
}

export interface UnsubscribeCommentsThreadCommand {
    threadId: number;
}

export interface GetSignInDataResult {
    avatar: string | null;
    title: string | null;
}

export interface ShelfDto {
    id: number;
    name: string;
    description: string;
    isDefault: boolean;
    isPublic: boolean;
    isQuickAdd: boolean;
    trackUpdates: boolean;
    color: string;
    storiesCount: number;
    iconName: string | null;
    iconId: number | null;
}

export interface CreateShelfCommand {
    name: string | null;
    description: string | null;
    isQuickAdd: boolean;
    isPublic: boolean;
    trackUpdates: boolean;
    color: string | null;
    icon: number;
}

export interface UpdateShelfCommand {
    id: number;
    name: string | null;
    description: string | null;
    isQuickAdd: boolean;
    isPublic: boolean;
    trackUpdates: boolean;
    color: string | null;
    icon: number;
}

export interface GetPaginatedUserShelvesResult {
    id: number;
    name: string | null;
    color: string | null;
    iconName: string | null;
    doesContainBook: boolean;
}

export interface AddBookToShelfResult {
    shelfId: number;
    storyId: number;
}

export interface RemoveBookFromShelfResult {
    shelfId: number;
    storyId: number;
}

export interface RoleDto {
    id: number;
    name: string;
    color: string | null;
    isStaff: boolean;
    order: number;
}

export interface UpdateRoleCommand {
    id: number;
    name: string | null;
    isStaff: boolean;
    color: string | null;
    order: number | null;
}

export interface CreateRoleCommand {
    name: string | null;
    isStaff: boolean;
    color: string | null;
    order: number | null;
}

export interface ReportContentCommand {
    itemId: number;
    reason: string | null;
    itemType: EReportableContentTypes;
}

export type EReportableContentTypes = "Comment" | "User" | "Story" | "Chapter" | "Blogpost" | "Club";

export interface RatingApiDto {
    id: number;
    name: string | null;
    description: string | null;
    order: number;
    icon: string | null;
    blacklistedByDefault: boolean;
}

export interface BaseModel {
    id: number;
}

export interface QuoteDto {
    body: string | null;
    author: string | null;
}

export interface CreateQuoteCommand {
    body: string | null;
    author: string | null;
}

export interface UpdateQuoteCommand {
    id: number;
    body: string | null;
    author: string | null;
}

export interface CreateQuotesFromJsonResponse {
    insertedRows: number;
}

export interface DeleteQuoteCommand {
    id: number;
}

export interface GetUserNotificationsResult {
    id: number;
    body: string | null;
    url: string | null;
    dateTime: string;
    event: ENotificationEvent;
    message: string | null;
}

export type ENotificationEvent = "System" | "WatchedStoryUpdated" | "WatchedThreadNewComment" | "FollowedAuthorNewBlogpost" | "FollowedAuthorNewStory" | "CommentReply";

export interface InviteCodeDto {
    id: number;
    code: string;
    normalizedCode: string;
    usedByUserName: string | null;
    issuedByUserName: string;
    issueDate: string;
    usedDate: string | null;
}

export interface GetUserInfractionsResult {
    id: number;
    activeUntil: string;
    removed: boolean;
    reason: string | null;
}

export interface GetInfractionDetailsResult {
    id: number;
    userName: string;
    userId: number;
    issueDate: string;
    activeUntil: string;
    removedAt: string | null;
    reason: string;
    type: InfractionType;
    issuedByName: string;
    removedByName: string | null;
}

export type InfractionType = "Note" | "Warning" | "Mute" | "Ban";

export interface CreateInfractionResponse {
    id: number;
    issuedBy: number;
    issuedAgainst: number;
}

export interface CreateInfractionCommand {
    userId: number;
    reason: string | null;
    endDate: string;
    type: InfractionType;
}

export interface DeactivateInfractionResponse {
    id: number;
    issuedBy: number;
    issuedAgainst: number;
}

export interface GetFolderResult {
    id: number;
    name: string | null;
    slug: string | null;
    parentFolderId: number | null;
    canAdd: boolean;
}

export interface FolderStory {
    folderId: number;
    storyId: number;
}

export interface ClubMember {
    memberId: number;
    clubId: number;
    role: EClubMemberRoles;
    memberSince: string;
}

export type EDeletedBy = "User" | "Staff";

export type EStoryStatus = "InProgress" | "Completed" | "OnHiatus" | "Cancelled";

export interface IdentityRoleOfLong {
    id: number;
    name: string | null;
    normalizedName: string | null;
    concurrencyStamp: string | null;
}

export interface IdentityUserRoleOfLong {
    userId: number;
    roleId: number;
}

export interface BlacklistedRating {
    userId: number;
    ratingId: number;
}

export interface BlacklistedTag {
    userId: number;
    tagId: number;
}

export interface IdentityUserOfLong {
    id: number;
    userName: string | null;
    normalizedUserName: string | null;
    email: string | null;
    normalizedEmail: string | null;
    emailConfirmed: boolean;
    passwordHash: string | null;
    securityStamp: string | null;
    concurrencyStamp: string | null;
    phoneNumber: string | null;
    phoneNumberConfirmed: boolean;
    twoFactorEnabled: boolean;
    lockoutEnd: string | null;
    lockoutEnabled: boolean;
    accessFailedCount: number;
}

export type EClubMemberRoles = "Founder" | "Admin" | "Moderator" | "User";

export interface AddStoryToFolderCommand {
    folderId: number;
    storyId: number;
}

export interface UpdateFaqCommand {
    id: number;
    question: string | null;
    answer: string | null;
}

export interface CreateFaqCommand {
    question: string | null;
    answer: string | null;
}

export interface CommandOfStory {
    objectId: number;
    reason: string | null;
}

export interface CommandOfChapter {
    objectId: number;
    reason: string | null;
}

export interface CommandOfBlogpost {
    objectId: number;
    reason: string | null;
}

export interface CommentsThreadControllerPermissionsResult {
    isSiteModerator: boolean;
    isClubModerator: boolean;
    isAllowed: boolean;
}

export interface CommentsThreadControllerPostData {
    id: number;
}

export interface PaginationResultOfCommentDto {
    elements: object[] | null;
    total: number;
    perPage: number;
    pages: number;
    page: number;
}

export interface CommentDto {
    id: number;
    author: UserSimpleDto;
    dateTime: string;
    lastEdit: string | null;
    editCount: number;
    owned: boolean;
    body: string;
    isBlocked: boolean;
}

export interface UserSimpleDto {
    userName: string | null;
    avatar: string | null;
    title: string | null;
    roles: object[] | null;
}

export interface GetRevisionResult {
    editTime: string;
    body: string | null;
}

export interface CommentsControllerPostData {
    body: string | null;
    thread: number;
    type: string | null;
}

export interface UpdateCommentCommand {
    body: string | null;
    id: number;
}

export interface GetJoinedClubsResponse {
    id: number;
    name: string | null;
    icon: string | null;
}

export interface GetClubsWithStoryResult {
    id: number;
    name: string | null;
    icon: string | null;
}

export interface BanUserCommand {
    userId: number;
    clubId: number;
    reason: string | null;
}

export interface UnbanUserCommand {
    userId: number;
    clubId: number;
}

export interface JoinClubCommand {
    clubId: number;
}

export interface LeaveClubCommand {
    clubId: number;
}

export interface MarkChapterAsReadCommand {
    chapter: number;
    story: number;
}

export interface MarkChapterAsUnreadCommand {
    chapter: number;
    story: number;
}