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
	owned: boolean;
	body: string | null;
	deletedBy: NullableOfEDeletedBy;
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
	namespace: NullableOfETagNamespace;
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

export interface GetClubsWithStoryResult {
	id: number;
	name: string;
	icon: string;
	folders: IEnumerableOfString;
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
	message: string | null;
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

export interface PaginationResultOfCommentDto {
	elements: IEnumerableOfCommentDto;
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
	namespace: NullableOfETagNamespace;
	namespaceColor: string | null;
	namespaceId: number | null;
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
	roles: IEnumerableOfInt64;
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
	namespace: NullableOfETagNamespace;
}

export interface UserSimpleDto {
	userName: string;
	avatar: string;
	title: string | null;
	roles: IEnumerableOfRoleDto;
}

export interface VoteResult {
	didVote: boolean;
	count: number | null;
}

export type AdminIssueInviteCodeCommand = Record<string, never>;

export type CommentSource = "Chapter" | "Blogpost" | "Profile" | "ForumPost";

export type DictionaryOfStringAndInt32 = Record<string, never>;

export type DictionaryOfStringAndUInt64 = Record<string, never>;

export type ENotificationEvent = "System" | "WatchedStoryUpdated" | "WatchedThreadNewComment" | "FollowedAuthorNewBlogpost" | "FollowedAuthorNewStory" | "CommentReply";

export type EReportableContentTypes = "Comment" | "User" | "Story" | "Chapter" | "Blogpost" | "Club";

export type HashSetOfInt64 = Record<string, never>;

export type IEnumerableOfCommentDto = Record<string, never>;

export type IEnumerableOfInt64 = Record<string, never>;

export type IEnumerableOfRoleDto = Record<string, never>;

export type IEnumerableOfString = Record<string, never>;

export type IFormFile = Record<string, never>;

export type InfractionType = "Note" | "Warning" | "Mute" | "Ban";

export type IssueInviteCodeCommand = Record<string, never>;

export type ListOfGetUserInfractionsResult = Record<string, never>;

export type ListOfGetUserNotificationsResult = Record<string, never>;

export type ListOfQuoteDto = Record<string, never>;

export type None = undefined;

export type NullableOfEDeletedBy = "User" | "Staff" | "null";

export type NullableOfETagNamespace = "ContentWarning" | "Genre" | "Franchise" | "null";