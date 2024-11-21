export interface Command {
	chapter: number;
	story: number;
}

export interface Command10 {
	body: string;
	author: string;
}

export interface Command11 {
	id: number;
	body: string;
	author: string;
}

export interface Command12 {
	itemId: number;
	reason: string;
	itemType: EReportableContentTypes;
}

export interface Command13 {
	name: string;
	isStaff: boolean;
	color: string;
	order: number;
}

export interface Command14 {
	id: number;
	name: string;
	isStaff: boolean;
	color: string;
	order: number;
}

export interface Command15 {
	shelfId: number;
	storyId: number;
}

export interface Command16 {
	name: string;
	description: string;
	isQuickAdd: boolean;
	isPublic: boolean;
	trackUpdates: boolean;
	color: string;
	icon: number;
}

export interface Command17 {
	id: number;
	name: string;
	description: string;
	isQuickAdd: boolean;
	isPublic: boolean;
	trackUpdates: boolean;
	color: string;
	icon: number;
}

export interface Command18 {
	name: string;
	description: string | null;
	namespace: NullableOfETagNamespace;
}

export interface Command19 {
	id: number;
	name: string | null;
	description: string | null;
	namespace: NullableOfETagNamespace;
}

export interface Command2 {
	clubId: number;
}

export interface Command20 {
	name: string;
}

export interface Command21 {
	userId: number;
	roles: number;
}

export interface Command22 {
	storyId: number;
}

export interface Command23 {
	userId: number;
	reason: string;
	endDate: string;
	type: InfractionType;
}

export interface Command24 {
	name: string;
	description: string;
	blacklistedByDefault: boolean;
	order: number;
	icon: IFormFile;
}

export interface Command25 {
	id: number;
	name: string;
	description: string;
	blacklistedByDefault: boolean;
	order: number;
	icon: IFormFile;
}

export interface Command3 {
	body: string;
	thread: number;
	source: CommentSource;
}

export interface Command4 {
	body: string;
	commentId: number;
}

export interface Command5 {
	threadId: number;
}

export interface Command6 {
	question: string;
	answer: string;
}

export interface Command7 {
	id: number;
	question: string;
	answer: string;
}

export interface Command8 {
	folderId: number;
	storyId: number;
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

export interface FaqDto {
	id: number;
	question: string;
	answer: string;
	answerRendered: string;
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

export interface PaginationResultOfCommentDto {
	elements: object;
	total: number;
	perPage: number;
	pages: number;
	page: number;
}

export interface Query {
	quotes: object;
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

export interface RemoveBookFromShelfResult {
	shelfId: number;
	storyId: number;
}

export interface Response {
	id: number;
	name: string;
	icon: string;
}

export interface Response2 {
	body: string;
	editTime: string;
}

export interface Response3 {
	folderId: number;
	storyId: number;
	added: string;
	addedById: number;
}

export interface Result {
	id: number;
	name: string;
	icon: string;
	folders: string[];
}

export interface Result2 {
	editTime: string;
	body: string;
}

export interface Result3 {
	perPage: number;
	minCommentLength: number;
	maxCommentLength: number;
	source: CommentSource;
	isLocked: boolean;
}

export interface Result4 {
	id: number;
	name: string;
	slug: string;
	canAdd: boolean;
}

export interface Result5 {
	id: number;
	body: string | null;
	url: string;
	dateTime: string;
	event: ENotificationEvent;
	message: string | null;
}

export interface Result6 {
	avatar: string;
	title: string | null;
}

export interface Result7 {
	id: number;
	activeUntil: string;
	removed: boolean;
	reason: string;
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

export interface TagDto {
	id: number;
	name: string;
	slug: string;
	description: string | null;
	namespace: NullableOfETagNamespace;
	namespaceColor: string | null;
	namespaceId: number | null;
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

export type Command9 = Record<string, never>;

export type CommentSource = "Chapter" | "Blogpost" | "Profile" | "ForumPost";

export type ENotificationEvent = "System" | "WatchedStoryUpdated" | "WatchedThreadNewComment" | "FollowedAuthorNewBlogpost" | "FollowedAuthorNewStory" | "CommentReply";

export type EReportableContentTypes = "Comment" | "User" | "Story" | "Chapter" | "Blogpost" | "Club";

export type IFormFile = Record<string, never>;

export type InfractionType = "Note" | "Warning" | "Mute" | "Ban";

export type None = undefined;

export type NullableOfEDeletedBy = "User" | "Staff" | "null";

export type NullableOfETagNamespace = "ContentWarning" | "Genre" | "Franchise" | "null";