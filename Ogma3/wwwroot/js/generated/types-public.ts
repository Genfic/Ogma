export type None = undefined;

export interface MarkChapterAsReadCommand {
    chapter: number;
    story: number;
}

export interface MarkChapterAsUnreadCommand {
    chapter: number;
    story: number;
}

export interface JoinClubCommand {
    clubId: number;
}

export interface LeaveClubCommand {
    clubId: number;
}

export interface FaqDto {
    id: number;
    question: string;
    answer: string;
    answerRendered: string;
}

export interface CreateFaqCommand {
    question: string;
    answer: string;
}

export interface UpdateFaqCommand {
    id: number;
    question: string;
    answer: string;
}

export interface InviteCodeDto {
    id: number;
    code: string;
    normalizedCode: string;
    usedByUserName: string | null;
    issuedByUserName: string;
    issueDate: string;
    usedDate: string | null;
}

export interface AdminIssueInviteCodeCommand {}

export interface IssueInviteCodeCommand {}

export interface GetUserNotificationsResult {
    id: number;
    body: string | null;
    url: string;
    dateTime: string;
    event: ENotificationEvent;
    message: string;
}

export interface ENotificationEvent {}

export interface QuoteDto {
    body: string;
    author: string;
}

export interface CreateQuoteCommand {
    body: string;
    author: string;
}

export interface UpdateQuoteCommand {
    id: number;
    body: string;
    author: string;
}

export interface ReportContentCommand {
    itemId: number;
    reason: string;
    itemType: EReportableContentTypes;
}

export interface EReportableContentTypes {}

export interface ShelfAddResult {
    shelfId: number;
    storyId: number;
}

export interface AddBookToShelfCommand {
    shelfId: number;
    storyId: number;
}

export interface QuickShelvesResult {
    id: number;
    name: string;
    color: string | null;
    iconName: string | null;
    doesContainBook: boolean;
}

export interface ShelfResult {
    id: number;
    name: string;
    color: string | null;
    iconName: string | null;
    doesContainBook: boolean;
}

export interface RemoveBookFromShelfResult {
    shelfId: number;
    storyId: number;
}

export interface RemoveBookFromShelfCommand {
    shelfId: number;
    storyId: number;
}

export interface ShelfDto {
    id: number;
    name: string;
    description: string;
    isDefault: boolean;
    isPublic: boolean;
    isQuickAdd: boolean;
    trackUpdates: boolean;
    color: string | null;
    storiesCount: number;
    iconName: string | null;
    iconId: number | null;
}

export interface CreateShelfCommand {
    name: string;
    description: string;
    isQuickAdd: boolean;
    isPublic: boolean;
    trackUpdates: boolean;
    color: string;
    icon: number;
}

export interface UpdateShelfCommand {
    id: number;
    name: string;
    description: string;
    isQuickAdd: boolean;
    isPublic: boolean;
    trackUpdates: boolean;
    color: string;
    icon: number;
}

export interface GetSignInDataResult {
    avatar: string;
    title: string | null;
}

export interface BlockUserCommand {
    name: string;
}

export interface FollowUserCommand {
    name: string;
}

export interface UnblockUserCommand {
    name: string;
}

export interface UnfollowUserCommand {
    name: string;
}

export interface UpdateRolesCommand {
    userId: number;
    roles: number;
}

export interface VoteResult {
    didVote: boolean;
    count: number | null;
}

export interface CreateVoteCommand {
    storyId: number;
}

export interface DeleteVoteCommand {
    storyId: number;
}

export interface ErrorControllerResult {
    code: number | null;
    reason: string;
}

export interface TagDto {
    id: number;
    name: string;
    slug: string;
    description: string;
    namespace: ETagNamespace | null;
    namespaceColor: string | null;
    namespaceId: number | null;
}

export interface ETagNamespace {}

export interface ProblemDetails {
    type: string | null;
    title: string | null;
    status: number | null;
    detail: string | null;
    instance: string | null;
}

export interface UpdateTagCommand {
    id: number;
    name: string | null;
    description: string | null;
    namespace: ETagNamespace | null;
}

export interface CreateTagCommand {
    name: string;
    description: string | null;
    namespace: ETagNamespace | null;
}

export interface SubscribeCommentsThreadCommand {
    threadId: number;
}

export interface UnsubscribeCommentsThreadCommand {
    threadId: number;
}

export interface RoleDto {
    id: number;
    name: string;
    color: string | null;
    isStaff: boolean;
    order: number | null;
}

export interface UpdateRoleCommand {
    id: number;
    name: string;
    isStaff: boolean;
    color: string;
    order: number | null;
}

export interface CreateRoleCommand {
    name: string;
    isStaff: boolean;
    color: string;
    order: number | null;
}

export interface RatingApiDto {
    id: number;
    name: string | null;
    description: string | null;
    order: number;
    icon: string | null;
    blacklistedByDefault: boolean;
}

export interface GetFolderResult {
    id: number;
    name: string;
    slug: string;
    parentFolderId: number | null;
    canAdd: boolean;
}

export interface AddStoryToFolderResponse {
    folderId: number;
    storyId: number;
    added: string;
    addedById: number;
}

export interface AddStoryToFolderCommand {
    folderId: number;
    storyId: number;
}

export interface GetPermissionsResult {
    isSiteModerator: boolean;
    isClubModerator: boolean;
    isAllowed: boolean;
}

export interface LockThreadCommand {
    id: number;
}

export interface PaginationResultOfCommentDto {
    elements: object;
    total: number;
    perPage: number;
    pages: number;
    page: number;
}

export interface CommentDto {
    id: number;
    author: UserSimpleDto | null;
    dateTime: string;
    lastEdit: string | null;
    editCount: number;
    owned: boolean;
    body: string;
    deletedBy: EDeletedBy | null;
    isBlocked: boolean;
}

export interface UserSimpleDto {
    userName: string;
    avatar: string;
    title: string | null;
    roles: object;
}

export interface EDeletedBy {}

export interface GetRevisionResult {
    editTime: string;
    body: string;
}

export interface CommentsControllerPostData {
    body: string;
    thread: number;
    type: string;
}

export interface UpdateCommentCommand {
    body: string;
    id: number;
}

export interface GetJoinedClubsResponse {
    id: number;
    name: string;
    icon: string;
}

export interface GetClubsWithStoryResult {
    id: number;
    name: string;
    icon: string;
}

export interface BanUserCommand {
    userId: number;
    clubId: number;
    reason: string;
}

export interface UnbanUserCommand {
    userId: number;
    clubId: number;
}