export interface AddBookToShelfCommand {
	shelfId: number;
	storyId: number;
}

export interface AddStoryToFolderCommand {
	folderId: number;
	storyId: number;
}

export interface AddStoryToFolderResponse {
	folderId: number;
	storyId: number;
	added: string;
	addedById: number;
}

export interface BanUserCommand {
	userId: number;
	clubId: number;
	reason: string;
}

export interface BlockUserCommand {
	name: string;
}

export interface CommentDto {
	id: number;
	author: UserSimpleDto | null;
	dateTime: string;
	owned: boolean;
	body: string | null;
	deletedBy: EDeletedBy | null;
	isBlocked: boolean;
	isEdited: boolean;
}

export interface CreateCommentCommand {
	body: string;
	thread: number;
	source: CommentSource;
}

export interface CreateFaqCommand {
	question: string;
	answer: string;
}

export interface CreateQuoteCommand {
	body: string;
	author: string;
}

export interface CreateQuotesFromJsonQuery {
	quotes: object;
}

export interface CreateRatingCommand {
	name: string;
	description: string;
	blacklistedByDefault: boolean;
	order: number;
	icon: string;
}

export interface CreateRoleCommand {
	name: string;
	isStaff: boolean;
	color: string;
	order: number;
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

export interface CreateTagCommand {
	name: string;
	description: string | null;
	namespace: ETagNamespace | null;
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

export interface FaqDto {
	id: number;
	question: string;
	answer: string;
	answerRendered: string;
}

export interface FollowUserCommand {
	name: string;
}

export interface GetClubsWithStoryResult {
	id: number;
	name: string;
	icon: string;
}

export interface GetFolderResult {
	id: number;
	name: string;
	slug: string;
	parentFolderId: number | null;
	canAdd: boolean;
}

export interface GetJoinedClubsResponse {
	id: number;
	name: string;
	icon: string;
}

export interface GetRevisionResult {
	editTime: string;
	body: string;
}

export interface GetSignInDataResult {
	avatar: string;
	title: string | null;
}

export interface GetThreadDetailsResult {
	perPage: number;
	minCommentLength: number;
	maxCommentLength: number;
	source: CommentSource;
	isLocked: boolean;
}

export interface GetUserNotificationsResult {
	id: number;
	body: string | null;
	url: string;
	dateTime: string;
	event: ENotificationEvent;
	message: string;
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

export interface JoinClubCommand {
	clubId: number;
}

export interface LeaveClubCommand {
	clubId: number;
}

export interface LockThreadCommand {
	threadId: number;
}

export interface MarkChapterAsReadCommand {
	chapter: number;
	story: number;
}

export interface MarkChapterAsUnreadCommand {
	chapter: number;
	story: number;
}

export interface PaginationResultOfCommentDto {
	elements: object;
	total: number;
	perPage: number;
	pages: number;
	page: number;
}

export interface QuickShelvesResult {
	id: number;
	name: string;
	color: string | null;
	iconName: string | null;
	doesContainBook: boolean;
}

export interface QuoteDto {
	body: string;
	author: string;
}

export interface RatingApiDto {
	id: number;
	name: string;
	description: string;
	order: number;
	icon: string;
	blacklistedByDefault: boolean;
}

export interface RemoveBookFromShelfCommand {
	shelfId: number;
	storyId: number;
}

export interface RemoveBookFromShelfResult {
	shelfId: number;
	storyId: number;
}

export interface ReportContentCommand {
	itemId: number;
	reason: string;
	itemType: EReportableContentTypes;
}

export interface RoleDto {
	id: number;
	name: string;
	color: string | null;
	isStaff: boolean;
	order: number;
}

export interface ShelfAddResult {
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

export interface ShelfResult {
	id: number;
	name: string;
	color: string | null;
	iconName: string | null;
	doesContainBook: boolean;
}

export interface SubscribeCommentsThreadCommand {
	threadId: number;
}

export interface TagDto {
	id: number;
	name: string;
	slug: string;
	description: string | null;
	namespace: ETagNamespace | null;
	namespaceColor: string | null;
	namespaceId: number | null;
}

export interface UnbanUserCommand {
	userId: number;
	clubId: number;
}

export interface UnblockUserCommand {
	name: string;
}

export interface UnfollowUserCommand {
	name: string;
}

export interface UnsubscribeCommentsThreadCommand {
	threadId: number;
}

export interface UpdateCommentCommand {
	body: string;
	commentId: number;
}

export interface UpdateCommentResponse {
	body: string;
	editTime: string;
}

export interface UpdateFaqCommand {
	id: number;
	question: string;
	answer: string;
}

export interface UpdateQuoteCommand {
	id: number;
	body: string;
	author: string;
}

export interface UpdateRatingCommand {
	id: number;
	name: string;
	description: string;
	blacklistedByDefault: boolean;
	order: number;
	icon: string;
}

export interface UpdateRoleCommand {
	id: number;
	name: string;
	isStaff: boolean;
	color: string;
	order: number;
}

export interface UpdateRolesCommand {
	userId: number;
	roles: number;
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

export interface UpdateTagCommand {
	id: number;
	name: string | null;
	description: string | null;
	namespace: ETagNamespace | null;
}

export interface UserSimpleDto {
	userName: string;
	avatar: string;
	title: string | null;
	roles: object;
}

export interface VoteResult {
	didVote: boolean;
	count: number | null;
}

export type AdminIssueInviteCodeCommand = Record<string, never>;

export type CommentSource = "Invalid" | "Chapter" | "Blogpost" | "Profile" | "ForumPost";

export type EDeletedBy = "User" | "Staff";

export type ENotificationEvent = "System" | "WatchedStoryUpdated" | "WatchedThreadNewComment" | "FollowedAuthorNewBlogpost" | "FollowedAuthorNewStory" | "CommentReply";

export type EReportableContentTypes = "Comment" | "User" | "Story" | "Chapter" | "Blogpost" | "Club";

export type ETagNamespace = "ContentWarning" | "Genre" | "Franchise";

export type IssueInviteCodeCommand = Record<string, never>;

export type None = undefined;