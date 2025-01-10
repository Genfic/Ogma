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

export interface BlockUserCommand {
	name: string;
}

export interface CommentDto {
	id: number;
	author: UserSimpleDto;
	dateTime: string;
	body: string | null;
	deletedBy: unknown;
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

export interface CreateInfractionCommand {
	userId: number;
	reason: string;
	endDate: string;
	type: InfractionType;
}

export interface CreateQuoteCommand {
	body: string;
	author: string;
}

export interface CreateQuotesFromJsonQuery {
	quotes: object;
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
	namespace: unknown;
}

export interface CreateVoteCommand {
	storyId: number;
}

export interface DeleteVoteCommand {
	storyId: number;
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

export interface FullQuoteDto {
	id: number;
	body: string;
	author: string;
}

export interface GetClubsWithStoryResult {
	id: number;
	name: string;
	icon: string;
	folders: string[];
}

export interface GetFolderResult {
	id: number;
	name: string;
	slug: string;
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

export interface GetUserInfractionsResult {
	id: number;
	activeUntil: string;
	removed: boolean;
	reason: string;
}

export interface GetUserNotificationsResult {
	id: number;
	body: string | null;
	url: string;
	dateTime: string;
	event: ENotificationEvent;
	message: string;
}

export interface InfractionDto {
	id: number;
	userUserName: string;
	userId: number;
	issueDate: string;
	activeUntil: string;
	removedAt: string | null;
	reason: string;
	type: InfractionType;
	issuedByUserName: string;
	removedByUserName: string | null;
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

export interface RoleTinyDto {
	name: string;
	color: string | null;
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
	color: string;
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
	namespace: unknown;
	namespaceColor: string;
	namespaceId: number;
}

export interface UnblockUserCommand {
	name: string;
}

export interface UnfollowUserCommand {
	name: string;
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
	namespace: unknown;
}

export interface UserSimpleDto {
	userName: string;
	avatar: string;
	title: string;
	roles: object;
}

export interface VoteResult {
	didVote: boolean;
	count: number;
}

export type AdminIssueInviteCodeCommand = Record<string, never>;

export type CommentSource = "Chapter" | "Blogpost" | "Profile" | "ForumPost";

export type ENotificationEvent = "System" | "WatchedStoryUpdated" | "WatchedThreadNewComment" | "FollowedAuthorNewBlogpost" | "FollowedAuthorNewStory" | "CommentReply";

export type EReportableContentTypes = "Comment" | "User" | "Story" | "Chapter" | "Blogpost" | "Club";

export type IFormFile = Blob;

export type InfractionType = "Note" | "Warning" | "Mute" | "Ban";

export type IssueInviteCodeCommand = Record<string, never>;

export type None = undefined;