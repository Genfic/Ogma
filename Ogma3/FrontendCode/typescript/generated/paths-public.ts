import { del, get, head, patch, post, put, typedFetch } from "./typed-fetch";
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
	LocateCommentResponse,
	LockThreadCommand,
	MarkChapterAsReadCommand,
	MarkChapterAsUnreadCommand,
	QuoteDto,
	RatingApiDto,
	RemoveBookFromShelfCommand,
	RemoveBookFromShelfResult,
	ReportContentCommand,
	RoleDto,
	ShelfDto,
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


export const DeleteApiChaptersread = async (body: MarkChapterAsUnreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[], MarkChapterAsUnreadCommand>("/api/chaptersread",
	del,
	body,
	headers,
	options,
);


export const DeleteApiClubjoin = async (body: LeaveClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean|string, LeaveClubCommand>("/api/clubjoin",
	del,
	body,
	headers,
	options,
);


export const DeleteApiComments = async (commentId: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string, undefined>(`/api/comments/${commentId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiFaqs = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/faqs?id=${id}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiInviteCodes = async (codeId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/InviteCodes/${codeId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiNotifications = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, undefined>(`/api/notifications/${id}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiQuotes = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/quotes/${id}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiRatings = async (ratingId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/ratings/${ratingId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiRoles = async (roleId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/roles?roleId=${roleId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiShelfStories = async (body: RemoveBookFromShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RemoveBookFromShelfResult, RemoveBookFromShelfCommand>("/api/ShelfStories",
	del,
	body,
	headers,
	options,
);


export const DeleteApiShelves = async (shelfId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/shelves/${shelfId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiSubscriptionsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, undefined>(`/api/subscriptions/thread?threadId=${threadId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiTags = async (tagId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>(`/api/tags?tagId=${tagId}`,
	del,
	undefined,
	headers,
	options,
);


export const DeleteApiUsersBlock = async (body: UnblockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, UnblockUserCommand>("/api/users/block",
	del,
	body,
	headers,
	options,
);


export const DeleteApiUsersFollow = async (body: UnfollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, UnfollowUserCommand>("/api/users/follow",
	del,
	body,
	headers,
	options,
);


export const DeleteApiVotes = async (body: DeleteVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult, DeleteVoteCommand>("/api/votes",
	del,
	body,
	headers,
	options,
);


export const GetAllQuotes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<FullQuoteDto[], undefined>("/api/quotes",
	get,
	undefined,
	headers,
	options,
);


export const GetApiChaptersRead = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[], undefined>(`/api/ChaptersRead/${id}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiClubJoin = async (clubId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, undefined>(`/api/ClubJoin/${clubId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiClubsStory = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetClubsWithStoryResult[], undefined>(`/api/clubs/story/${storyId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiClubsUser = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetJoinedClubsResponse[], undefined>("/api/clubs/user",
	get,
	undefined,
	headers,
	options,
);


export const GetApiComments = async (thread: number, page: number | null, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{
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
}, undefined>(`/api/comments?thread=${thread}&page=${page}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiCommentsLocate = async (threadId: number, commentId: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<LocateCommentResponse, undefined>(`/api/comments/locate?threadId=${threadId}&commentId=${commentId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiCommentsMd = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string, undefined>(`/api/comments/${commentId}/md`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiCommentsRevisions = async (commentId: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetRevisionResult[], undefined>(`/api/comments/${commentId}/revisions`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiCommentsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetThreadDetailsResult, undefined>(`/api/CommentsThread/${threadId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiFaqs = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto[], undefined>("/api/faqs",
	get,
	undefined,
	headers,
	options,
);


export const GetApiFolders = async (clubId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetFolderResult[], undefined>(`/api/folders?clubId=${clubId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[], undefined>("/api/InviteCodes",
	get,
	undefined,
	headers,
	options,
);


export const GetApiInviteCodesPaginated = async (page: number, perPage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[], undefined>(`/api/InviteCodes/paginated?page=${page}&perPage=${perPage}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiNotifications = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserNotificationsResult[], undefined>("/api/notifications",
	get,
	undefined,
	headers,
	options,
);


export const GetApiNotificationsCount = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>("/api/notifications/count",
	get,
	undefined,
	headers,
	options,
);


export const GetApiQuotesRandom = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto, undefined>("/api/quotes/random",
	get,
	undefined,
	headers,
	options,
);


export const GetApiRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto[], undefined>("/api/ratings",
	get,
	undefined,
	headers,
	options,
);


export const GetApiRoles = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto[], undefined>("/api/roles",
	get,
	undefined,
	headers,
	options,
);


export const GetApiShelfStories = async (storyId: number, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetPaginatedUserShelvesResult[], undefined>(`/api/ShelfStories/${storyId}?page=${page}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiShelfStoriesQuick = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetCurrentUserQuickShelvesResult[], undefined>(`/api/ShelfStories/${storyId}/quick`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiShelves = async (userName: string, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto[], undefined>(`/api/shelves/${userName}?page=${page}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiSignin = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetSignInDataResult, undefined>(`/api/signin?name=${name}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiSubscriptionsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, undefined>(`/api/subscriptions/thread?threadId=${threadId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiTags = async (page: number, perPage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[], undefined>(`/api/tags?page=${page}&perPage=${perPage}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiTagsAll = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[], undefined>("/api/tags/all",
	get,
	undefined,
	headers,
	options,
);


export const GetApiTagsSearch = async (searchString: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[], undefined>(`/api/tags/search?searchString=${searchString}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiTagsStory = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[], undefined>(`/api/tags/story/${storyId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiTest = async (q: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, undefined>(`/api/test?q=${q}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiUsersNames = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string[]|string, undefined>(`/api/users/names?name=${name}`,
	get,
	undefined,
	headers,
	options,
);


export const GetApiVotes = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult, undefined>(`/api/votes/${storyId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetComment = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<CommentDto, undefined>(`/api/comments/${commentId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetRatingById = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto, undefined>(`/api/ratings/${id}`,
	get,
	undefined,
	headers,
	options,
);


export const GetRoleById = async (roleId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto, undefined>(`/api/roles/${roleId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetShelf = async (shelfId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto, undefined>(`/api/shelves/${shelfId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetSingleFaq = async (faqId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto, undefined>(`/api/faqs/${faqId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetSingleQuote = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto, undefined>(`/api/quotes/${id}`,
	get,
	undefined,
	headers,
	options,
);


export const GetSingleTag = async (tagId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto, undefined>(`/api/tags/${tagId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetTagNamespaces = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetTagNamespacesNamespaceDto[], undefined>("/api/tags/namespaces",
	get,
	undefined,
	headers,
	options,
);


export const PatchApiComments = async (body: UpdateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<UpdateCommentResponse, UpdateCommentCommand>("/api/comments",
	patch,
	body,
	headers,
	options,
);


export const PostApiChaptersread = async (body: MarkChapterAsReadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[], MarkChapterAsReadCommand>("/api/chaptersread",
	post,
	body,
	headers,
	options,
);


export const PostApiClubjoin = async (body: JoinClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, JoinClubCommand>("/api/clubjoin",
	post,
	body,
	headers,
	options,
);


export const PostApiComments = async (body: CreateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string, CreateCommentCommand>("/api/comments",
	post,
	body,
	headers,
	options,
);


export const PostApiCommentsThreadLock = async (body: LockThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, LockThreadCommand>("/api/CommentsThread/lock",
	post,
	body,
	headers,
	options,
);


export const PostApiFaqs = async (body: CreateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto, CreateFaqCommand>("/api/faqs",
	post,
	body,
	headers,
	options,
);


export const PostApiFoldersAddStory = async (body: AddStoryToFolderCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<AddStoryToFolderResponse|string, AddStoryToFolderCommand>("/api/folders/AddStory",
	post,
	body,
	headers,
	options,
);


export const PostApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto|string, IssueInviteCodeCommand>("/api/InviteCodes",
	post,
	undefined,
	headers,
	options,
);


export const PostApiInviteCodesNoLimit = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto, AdminIssueInviteCodeCommand>("/api/InviteCodes/no-limit",
	post,
	undefined,
	headers,
	options,
);


export const PostApiQuotes = async (body: CreateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FullQuoteDto, CreateQuoteCommand>("/api/quotes",
	post,
	body,
	headers,
	options,
);


export const PostApiQuotesJson = async (body: CreateQuotesFromJsonQuery, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, CreateQuotesFromJsonQuery>("/api/quotes/json",
	post,
	body,
	headers,
	options,
);


export const PostApiRatings = async (body: CreateRatingCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto, CreateRatingCommand>("/api/ratings",
	post,
	body,
	headers,
	options,
);


export const PostApiReports = async (body: ReportContentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, ReportContentCommand>("/api/reports",
	post,
	body,
	headers,
	options,
);


export const PostApiRoles = async (body: CreateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto|string, CreateRoleCommand>("/api/roles",
	post,
	body,
	headers,
	options,
);


export const PostApiShelfStories = async (body: AddBookToShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<AddBookToShelfResult, AddBookToShelfCommand>("/api/ShelfStories",
	post,
	body,
	headers,
	options,
);


export const PostApiShelves = async (body: CreateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto, CreateShelfCommand>("/api/shelves",
	post,
	body,
	headers,
	options,
);


export const PostApiSubscriptionsThread = async (body: SubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, SubscribeCommentsThreadCommand>("/api/subscriptions/thread",
	post,
	body,
	headers,
	options,
);


export const PostApiTags = async (body: CreateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto|string, CreateTagCommand>("/api/tags",
	post,
	body,
	headers,
	options,
);


export const PostApiUsersBlock = async (body: BlockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, BlockUserCommand>("/api/users/block",
	post,
	body,
	headers,
	options,
);


export const PostApiUsersFollow = async (body: FollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean, FollowUserCommand>("/api/users/follow",
	post,
	body,
	headers,
	options,
);


export const PostApiUsersRoles = async (body: UpdateRolesCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, UpdateRolesCommand>("/api/users/roles",
	post,
	body,
	headers,
	options,
);


export const PostApiVotes = async (body: CreateVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult, CreateVoteCommand>("/api/votes",
	post,
	body,
	headers,
	options,
);


export const PostHooksPatreon = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<string, undefined>("/hooks/patreon",
	post,
	undefined,
	headers,
	options,
);


export const PutApiFaqs = async (body: UpdateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, UpdateFaqCommand>("/api/faqs",
	put,
	body,
	headers,
	options,
);


export const PutApiQuotes = async (body: UpdateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, UpdateQuoteCommand>("/api/quotes",
	put,
	body,
	headers,
	options,
);


export const PutApiRatings = async (body: UpdateRatingCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, UpdateRatingCommand>("/api/ratings",
	put,
	body,
	headers,
	options,
);


export const PutApiRoles = async (body: UpdateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, UpdateRoleCommand>("/api/roles",
	put,
	body,
	headers,
	options,
);


export const PutApiShelves = async (body: UpdateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, UpdateShelfCommand>("/api/shelves",
	put,
	body,
	headers,
	options,
);


export const PutApiTags = async (body: UpdateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string, UpdateTagCommand>("/api/tags",
	put,
	body,
	headers,
	options,
);


export const UpdateLastActive = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>("/api/useractivity",
	head,
	undefined,
	headers,
	options,
);

