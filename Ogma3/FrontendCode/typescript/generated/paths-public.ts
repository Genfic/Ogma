import { DELETE, GET, HEAD, PATCH, POST, PUT, typedFetch } from "./typed-fetch";
import type {
	AddBookToShelfCommand,
	AddBookToShelfResult,
	AddStoryToFolderCommand,
	AddStoryToFolderResponse,
	AdminIssueInviteCodeCommand,
	BlockUserCommand,
	CommentDto,
	CreateCommentCommand,
	CreateFaqCommand,
	CreateQuoteCommand,
	CreateQuotesFromJsonQuery,
	CreateRatingCommand,
	CreateRoleCommand,
	CreateShelfCommand,
	CreateTagCommand,
	CreateVoteCommand,
	DeleteVoteCommand,
	FaqDto,
	FollowUserCommand,
	FullQuoteDto,
	GetClubsWithStoryResult,
	GetCurrentUserQuickShelvesResult,
	GetFolderResult,
	GetJoinedClubsResponse,
	GetPaginatedUserShelvesResult,
	GetRevisionResult,
	GetSignInDataResult,
	GetTagNamespacesNamespaceDto,
	GetThreadDetailsResult,
	GetUserNotificationsResult,
	InviteCodeDto,
	IssueInviteCodeCommand,
	JoinClubCommand,
	LeaveClubCommand,
	ListPasskeysUserPasskey,
	LocateCommentResponse,
	LockThreadCommand,
	MarkChapterAsReadCommand,
	MarkChapterAsUnreadCommand,
	QuoteDto,
	RatingApiDto,
	RegisterPasskeyQuery,
	RegisterPasskeyResponse,
	RemoveBookFromShelfCommand,
	RemoveBookFromShelfResult,
	ReportContentCommand,
	RoleDto,
	ShelfDto,
	SignInWithPasskeyQuery,
	SubscribeCommentsThreadCommand,
	TagDto,
	UnblockUserCommand,
	UnfollowUserCommand,
	UpdateCommentCommand,
	UpdateCommentResponse,
	UpdateFaqCommand,
	UpdateQuoteCommand,
	UpdateRatingCommand,
	UpdateRoleCommand,
	UpdateRolesCommand,
	UpdateShelfCommand,
	UpdateTagCommand,
	VoteResult,
} from "./types-public";

const _enc = <T>(p: T): T extends string ? string : T => (typeof p === 'string' ? encodeURIComponent(p) : p) as any;


export const DeleteApiChaptersread = async (body: MarkChapterAsUnreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number[]; 204: undefined; 400: undefined; 401: undefined }, MarkChapterAsUnreadCommand>("/api/chaptersread",
    DELETE,
    body,
    headers,
    options,
);

export const DeleteApiClubjoin = async (body: LeaveClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined }, LeaveClubCommand>("/api/clubjoin",
    DELETE,
    body,
    headers,
    options,
);

export const DeleteApiComments = async (commentId: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: string; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/comments/${commentId}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiFaqs = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/faqs?id=${_enc(id)}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiInviteCodes = async (codeId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/InviteCodes/${codeId}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiNotifications = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/notifications/${id}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiPasskeysDelete = async (id: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 401: undefined; 404: undefined; 500: undefined }, undefined>(`/api/passkeys/delete?id=${_enc(id)}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiQuotes = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/quotes/${id}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiRatings = async (ratingId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/ratings/${ratingId}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiRoles = async (roleId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/roles?roleId=${_enc(roleId)}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiShelfStories = async (body: RemoveBookFromShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: RemoveBookFromShelfResult; 400: undefined; 401: undefined; 404: undefined }, RemoveBookFromShelfCommand>("/api/ShelfStories",
    DELETE,
    body,
    headers,
    options,
);

export const DeleteApiShelves = async (shelfId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/shelves/${shelfId}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiSubscriptionsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined }, undefined>(`/api/subscriptions/thread?threadId=${_enc(threadId)}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiTags = async (tagId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/tags?tagId=${_enc(tagId)}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteApiUsersBlock = async (body: UnblockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined; 404: undefined }, UnblockUserCommand>("/api/users/block",
    DELETE,
    body,
    headers,
    options,
);

export const DeleteApiUsersFollow = async (body: UnfollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined; 404: undefined }, UnfollowUserCommand>("/api/users/follow",
    DELETE,
    body,
    headers,
    options,
);

export const DeleteApiVotes = async (body: DeleteVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: VoteResult; 400: undefined; 401: undefined; 404: undefined }, DeleteVoteCommand>("/api/votes",
    DELETE,
    body,
    headers,
    options,
);

export const GetAllQuotes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: FullQuoteDto[] }, undefined>("/api/quotes",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiChaptersRead = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number[]; 400: undefined }, undefined>(`/api/ChaptersRead/${id}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiClubJoin = async (clubId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined }, undefined>(`/api/ClubJoin/${clubId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiClubsStory = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetClubsWithStoryResult[]; 400: undefined }, undefined>(`/api/clubs/story/${storyId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiClubsUser = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetJoinedClubsResponse[] }, undefined>("/api/clubs/user",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiComments = async (thread: number, page: number | null, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: {
    /** List of the fetched elements */
    elements: CommentDto[];
    /** Total number of elements */
    total: number;
    /** Number of elements per page */
    perPage: number;
    /** Number of pages the total number of elements can be divided into */
    pages: number;
    /** The requested page */
    page: number;
}; 304: undefined; 400: undefined }, undefined>(`/api/comments?thread=${_enc(thread)}&page=${_enc(page)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiCommentsLocate = async (threadId: number, commentId: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: LocateCommentResponse; 404: undefined }, undefined>(`/api/comments/locate?threadId=${_enc(threadId)}&commentId=${_enc(commentId)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiCommentsMd = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: string; 400: undefined; 404: undefined }, undefined>(`/api/comments/${commentId}/md`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiCommentsRevisions = async (commentId: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetRevisionResult[]; 400: undefined; 404: undefined }, undefined>(`/api/comments/${commentId}/revisions`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiCommentsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetThreadDetailsResult; 400: undefined; 404: undefined }, undefined>(`/api/CommentsThread/${threadId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiFaqs = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: FaqDto[] }, undefined>("/api/faqs",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiFolders = async (clubId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetFolderResult[]; 400: undefined }, undefined>(`/api/folders?clubId=${_enc(clubId)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: InviteCodeDto[]; 401: undefined }, undefined>("/api/InviteCodes",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiInviteCodesPaginated = async (page: number, perPage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: InviteCodeDto[]; 400: undefined; 401: undefined }, undefined>(`/api/InviteCodes/paginated?page=${_enc(page)}&perPage=${_enc(perPage)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiNotifications = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetUserNotificationsResult[]; 401: undefined }, undefined>("/api/notifications",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiNotificationsCount = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 204: undefined; 401: undefined }, undefined>("/api/notifications/count",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiPasskeysList = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: ListPasskeysUserPasskey[]; 401: undefined; 404: undefined; 500: undefined }, undefined>("/api/passkeys/list",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiPasskeysOptions = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: string; 401: undefined; 404: undefined; 500: undefined }, undefined>("/api/passkeys/options",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiPasskeysRequestOptions = async (username: string | null, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: string; 404: undefined; 500: undefined }, undefined>(`/api/passkeys/request-options?username=${_enc(username)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiQuotesRandom = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: QuoteDto; 404: undefined; 429: undefined }, undefined>("/api/quotes/random",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: RatingApiDto[] }, undefined>("/api/ratings",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiRoles = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: RoleDto[] }, undefined>("/api/roles",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiShelfStories = async (storyId: number, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetPaginatedUserShelvesResult[]; 400: undefined; 401: undefined }, undefined>(`/api/ShelfStories/${storyId}?page=${_enc(page)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiShelfStoriesQuick = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetCurrentUserQuickShelvesResult[]; 400: undefined; 401: undefined }, undefined>(`/api/ShelfStories/${storyId}/quick`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiShelves = async (userName: string, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: ShelfDto[]; 400: undefined; 401: undefined }, undefined>(`/api/shelves/${userName}?page=${_enc(page)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiSignin = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetSignInDataResult; 400: undefined }, undefined>(`/api/signin?name=${_enc(name)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiSubscriptionsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined }, undefined>(`/api/subscriptions/thread?threadId=${_enc(threadId)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiTags = async (page: number, perPage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: TagDto[]; 400: undefined }, undefined>(`/api/tags?page=${_enc(page)}&perPage=${_enc(perPage)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiTagsAll = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: TagDto[]; 401: undefined }, undefined>("/api/tags/all",
    GET,
    undefined,
    headers,
    options,
);

export const GetApiTagsSearch = async (searchString: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: TagDto[]; 400: undefined }, undefined>(`/api/tags/search?searchString=${_enc(searchString)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiTagsStory = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: TagDto[]; 400: undefined; 404: undefined }, undefined>(`/api/tags/story/${storyId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiTest = async (q: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined }, undefined>(`/api/test?q=${_enc(q)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiUsersNames = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: string[]; 400: undefined; 401: undefined; 422: string }, undefined>(`/api/users/names?name=${_enc(name)}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetApiVotes = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: VoteResult; 400: undefined }, undefined>(`/api/votes/${storyId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetComment = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: CommentDto; 400: undefined; 404: undefined }, undefined>(`/api/comments/${commentId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetRatingById = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: RatingApiDto; 400: undefined; 404: undefined }, undefined>(`/api/ratings/${id}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetRoleById = async (roleId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: RoleDto; 400: undefined; 404: undefined }, undefined>(`/api/roles/${roleId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetShelf = async (shelfId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: ShelfDto; 400: undefined; 401: undefined; 404: undefined }, undefined>(`/api/shelves/${shelfId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetSingleFaq = async (faqId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: FaqDto; 400: undefined; 404: undefined }, undefined>(`/api/faqs/${faqId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetSingleQuote = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: QuoteDto; 400: undefined; 404: undefined }, undefined>(`/api/quotes/${id}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetSingleTag = async (tagId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: TagDto; 400: undefined; 404: undefined }, undefined>(`/api/tags/${tagId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetTagNamespaces = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetTagNamespacesNamespaceDto[] }, undefined>("/api/tags/namespaces",
    GET,
    undefined,
    headers,
    options,
);

export const PatchApiComments = async (body: UpdateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: UpdateCommentResponse; 400: undefined; 401: undefined; 404: undefined }, UpdateCommentCommand>("/api/comments",
    PATCH,
    body,
    headers,
    options,
);

export const PostApiChaptersread = async (body: MarkChapterAsReadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number[]; 400: undefined; 401: undefined }, MarkChapterAsReadCommand>("/api/chaptersread",
    POST,
    body,
    headers,
    options,
);

export const PostApiClubjoin = async (body: JoinClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined; 404: undefined }, JoinClubCommand>("/api/clubjoin",
    POST,
    body,
    headers,
    options,
);

export const PostApiComments = async (body: CreateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: string; 400: undefined; 401: undefined; 404: undefined }, CreateCommentCommand>("/api/comments",
    POST,
    body,
    headers,
    options,
);

export const PostApiCommentsThreadLock = async (body: LockThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined; 404: undefined }, LockThreadCommand>("/api/CommentsThread/lock",
    POST,
    body,
    headers,
    options,
);

export const PostApiFaqs = async (body: CreateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 201: FaqDto; 400: undefined; 401: undefined }, CreateFaqCommand>("/api/faqs",
    POST,
    body,
    headers,
    options,
);

export const PostApiFoldersAddStory = async (body: AddStoryToFolderCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: AddStoryToFolderResponse; 400: undefined; 404: string; 409: string }, AddStoryToFolderCommand>("/api/folders/AddStory",
    POST,
    body,
    headers,
    options,
);

export const PostApiInviteCodes = async (body: IssueInviteCodeCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: InviteCodeDto; 400: string; 401: undefined }, IssueInviteCodeCommand>("/api/InviteCodes",
    POST,
    body,
    headers,
    options,
);

export const PostApiInviteCodesNoLimit = async (body: AdminIssueInviteCodeCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: InviteCodeDto; 401: undefined }, AdminIssueInviteCodeCommand>("/api/InviteCodes/no-limit",
    POST,
    body,
    headers,
    options,
);

export const PostApiPasskeysRegister = async (body: RegisterPasskeyQuery, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: RegisterPasskeyResponse; 400: string[]; 401: undefined; 404: undefined; 500: undefined }, RegisterPasskeyQuery>("/api/passkeys/register",
    POST,
    body,
    headers,
    options,
);

export const PostApiPasskeysSignin = async (body: SignInWithPasskeyQuery, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 401: undefined }, SignInWithPasskeyQuery>("/api/passkeys/signin",
    POST,
    body,
    headers,
    options,
);

export const PostApiQuotes = async (body: CreateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 201: FullQuoteDto; 400: undefined; 401: undefined }, CreateQuoteCommand>("/api/quotes",
    POST,
    body,
    headers,
    options,
);

export const PostApiQuotesJson = async (body: CreateQuotesFromJsonQuery, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined }, CreateQuotesFromJsonQuery>("/api/quotes/json",
    POST,
    body,
    headers,
    options,
);

export const PostApiRatings = async (body: CreateRatingCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 201: RatingApiDto; 400: undefined; 401: undefined }, CreateRatingCommand>("/api/ratings",
    POST,
    body,
    headers,
    options,
);

export const PostApiReports = async (body: ReportContentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 400: undefined; 401: undefined; 429: undefined }, ReportContentCommand>("/api/reports",
    POST,
    body,
    headers,
    options,
);

export const PostApiRoles = async (body: CreateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 201: RoleDto; 400: undefined; 401: undefined; 409: string }, CreateRoleCommand>("/api/roles",
    POST,
    body,
    headers,
    options,
);

export const PostApiShelfStories = async (body: AddBookToShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: AddBookToShelfResult; 400: undefined; 401: undefined; 404: undefined }, AddBookToShelfCommand>("/api/ShelfStories",
    POST,
    body,
    headers,
    options,
);

export const PostApiShelves = async (body: CreateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 201: ShelfDto; 400: undefined; 401: undefined }, CreateShelfCommand>("/api/shelves",
    POST,
    body,
    headers,
    options,
);

export const PostApiSubscriptionsThread = async (body: SubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined }, SubscribeCommentsThreadCommand>("/api/subscriptions/thread",
    POST,
    body,
    headers,
    options,
);

export const PostApiTags = async (body: CreateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 201: TagDto; 400: undefined; 401: undefined; 409: string }, CreateTagCommand>("/api/tags",
    POST,
    body,
    headers,
    options,
);

export const PostApiUsersBlock = async (body: BlockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined; 404: undefined }, BlockUserCommand>("/api/users/block",
    POST,
    body,
    headers,
    options,
);

export const PostApiUsersFollow = async (body: FollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: boolean; 400: undefined; 401: undefined; 404: undefined }, FollowUserCommand>("/api/users/follow",
    POST,
    body,
    headers,
    options,
);

export const PostApiUsersRoles = async (body: UpdateRolesCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, UpdateRolesCommand>("/api/users/roles",
    POST,
    body,
    headers,
    options,
);

export const PostApiVotes = async (body: CreateVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: VoteResult; 400: undefined; 401: undefined }, CreateVoteCommand>("/api/votes",
    POST,
    body,
    headers,
    options,
);

export const PostHooksPatreon = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 404: undefined; 500: undefined }, undefined>("/hooks/patreon",
    POST,
    undefined,
    headers,
    options,
);

export const PutApiFaqs = async (body: UpdateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, UpdateFaqCommand>("/api/faqs",
    PUT,
    body,
    headers,
    options,
);

export const PutApiQuotes = async (body: UpdateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, UpdateQuoteCommand>("/api/quotes",
    PUT,
    body,
    headers,
    options,
);

export const PutApiRatings = async (body: UpdateRatingCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, UpdateRatingCommand>("/api/ratings",
    PUT,
    body,
    headers,
    options,
);

export const PutApiRoles = async (body: UpdateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, UpdateRoleCommand>("/api/roles",
    PUT,
    body,
    headers,
    options,
);

export const PutApiShelves = async (body: UpdateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined }, UpdateShelfCommand>("/api/shelves",
    PUT,
    body,
    headers,
    options,
);

export const PutApiTags = async (body: UpdateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 400: undefined; 401: undefined; 404: undefined; 409: string }, UpdateTagCommand>("/api/tags",
    PUT,
    body,
    headers,
    options,
);

export const UpdateLastActive = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 204: undefined }, undefined>("/api/useractivity",
    HEAD,
    undefined,
    headers,
    options,
);
