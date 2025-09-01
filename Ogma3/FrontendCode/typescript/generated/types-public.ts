export type AddBookToShelfCommand = {
    shelfId: number;
    storyId: number;
};

export type AddBookToShelfResult = {
    shelfId: number;
    storyId: number;
};

export type AddStoryToFolderCommand = {
    folderId: number;
    storyId: number;
};

export type AddStoryToFolderResponse = {
    folderId: number;
    storyId: number;
    added: Date;
    addedById: number;
};

export type AdminIssueInviteCodeCommand = object;

export type BlockUserCommand = {
    name: string;
};

export type CommentDto = {
    id: number;
    author: UserSimpleDto;
    dateTime: Date;
    body: string | null;
    deletedBy: "User" | "Staff" | null;
    isBlocked: boolean;
    isEdited: boolean;
};

export type CommentSource = "Chapter" | "Blogpost" | "Profile" | "ForumPost";

export type CreateCommentCommand = {
    body: string;
    thread: number;
    source: CommentSource;
};

export type CreateFaqCommand = {
    question: string;
    answer: string;
};

export type CreateQuoteCommand = {
    body: string;
    author: string;
};

export type CreateQuotesFromJsonQuery = {
    quotes: QuoteDto[];
};

export type CreateRatingCommand = {
    name: string;
    description: string;
    blacklistedByDefault: boolean;
    order: number;
    color: string;
};

export type CreateRoleCommand = {
    name: string;
    isStaff: boolean;
    color: string | null;
    order: number;
};

export type CreateShelfCommand = {
    name: string;
    description: string;
    isQuickAdd: boolean;
    isPublic: boolean;
    trackUpdates: boolean;
    color: string;
    iconId: number;
};

export type CreateTagCommand = {
    name: string;
    description: string | null;
    namespace: "ContentWarning" | "Genre" | "Franchise" | null;
};

export type CreateVoteCommand = {
    storyId: number;
};

export type DeleteVoteCommand = {
    storyId: number;
};

export type ENotificationEvent = "System" | "WatchedStoryUpdated" | "WatchedThreadNewComment" | "FollowedAuthorNewBlogpost" | "FollowedAuthorNewStory" | "CommentReply";

export type EReportableContentTypes = "Comment" | "User" | "Story" | "Chapter" | "Blogpost" | "Club";

export type FaqDto = {
    id: number;
    question: string;
    answer: string;
    answerRendered: string;
};

export type FollowUserCommand = {
    name: string;
};

export type FullQuoteDto = {
    id: number;
    body: string;
    author: string;
};

export type GetClubsWithStoryResult = {
    id: number;
    name: string;
    icon: string;
    folders: string[];
};

export type GetCurrentUserQuickShelvesResult = {
    id: number;
    name: string;
    color: string | null;
    iconName: string | null;
    doesContainBook: boolean;
};

export type GetFolderResult = {
    id: number;
    name: string;
    slug: string;
    canAdd: boolean;
};

export type GetJoinedClubsResponse = {
    id: number;
    name: string;
    icon: string;
};

export type GetPaginatedUserShelvesResult = {
    id: number;
    name: string;
    color: string | null;
    iconName: string | null;
    doesContainBook: boolean;
};

export type GetRevisionResult = {
    editTime: Date;
    body: string;
};

export type GetSignInDataResult = {
    avatar: string;
    title: string | null;
};

export type GetTagNamespacesNamespaceDto = {
    value: number;
    name: string;
};

export type GetThreadDetailsResult = {
    perPage: number;
    minCommentLength: number;
    maxCommentLength: number;
    source: CommentSource;
    isLocked: boolean;
};

export type GetUserNotificationsResult = {
    id: number;
    body: string | null;
    url: string;
    dateTime: Date;
    event: ENotificationEvent;
    message: string;
};

export type InviteCodeDto = {
    id: number;
    code: string;
    normalizedCode: string;
    usedByUserName: string | null;
    issuedByUserName: string;
    issueDate: Date;
    usedDate: Date | null;
};

export type IssueInviteCodeCommand = object;

export type JoinClubCommand = {
    clubId: number;
};

export type LeaveClubCommand = {
    clubId: number;
};

export type LockThreadCommand = {
    threadId: number;
};

export type MarkChapterAsReadCommand = {
    chapter: number;
    story: number;
};

export type MarkChapterAsUnreadCommand = {
    chapter: number;
    story: number;
};

export type None = undefined;

export type QuoteDto = {
    body: string;
    author: string;
};

export type RatingApiDto = {
    id: number;
    name: string;
    description: string;
    order: number;
    blacklistedByDefault: boolean;
    color: string;
};

export type RemoveBookFromShelfCommand = {
    shelfId: number;
    storyId: number;
};

export type RemoveBookFromShelfResult = {
    shelfId: number;
    storyId: number;
};

export type ReportContentCommand = {
    itemId: number;
    reason: string;
    itemType: EReportableContentTypes;
};

export type RoleDto = {
    id: number;
    name: string;
    color: string | null;
    isStaff: boolean;
    order: number;
};

export type RoleTinyDto = {
    name: string;
    color: string | null;
    order: number;
};

export type ShelfDto = {
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
};

export type SubscribeCommentsThreadCommand = {
    threadId: number;
};

export type TagDto = {
    id: number;
    name: string;
    slug: string;
    description: string | null;
    namespace: "ContentWarning" | "Genre" | "Franchise";
    namespaceColor: string;
    namespaceId: number;
};

export type UnblockUserCommand = {
    name: string;
};

export type UnfollowUserCommand = {
    name: string;
};

export type UpdateCommentCommand = {
    body: string;
    commentId: number;
};

export type UpdateCommentResponse = {
    body: string;
    editTime: Date;
};

export type UpdateFaqCommand = {
    id: number;
    question: string;
    answer: string;
};

export type UpdateQuoteCommand = {
    id: number;
    body: string;
    author: string;
};

export type UpdateRatingCommand = {
    id: number;
    name: string;
    description: string;
    blacklistedByDefault: boolean;
    order: number;
    color: string;
};

export type UpdateRoleCommand = {
    id: number;
    name: string;
    isStaff: boolean;
    color: string | null;
    order: number;
};

export type UpdateRolesCommand = {
    userId: number;
    roles: number[];
};

export type UpdateShelfCommand = {
    id: number;
    name: string;
    description: string;
    isQuickAdd: boolean;
    isPublic: boolean;
    trackUpdates: boolean;
    color: string;
    iconId: number;
};

export type UpdateTagCommand = {
    id: number;
    name: string;
    description: string | null;
    namespace: "ContentWarning" | "Genre" | "Franchise" | null;
};

export type UserSimpleDto = {
    userName: string;
    avatar: string;
    title: string;
    roles: RoleTinyDto[];
} | null;

export type VoteResult = {
    didVote: boolean;
    count: number;
};