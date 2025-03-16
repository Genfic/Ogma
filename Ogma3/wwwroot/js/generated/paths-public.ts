import type {
	AddBookToShelfCommand,
	AddStoryToFolderCommand,
	AddStoryToFolderResponse,
	AdminIssueInviteCodeCommand,
	BlockUserCommand,
	CommentDto,
	CreateCommentCommand,
	CreateFaqCommand,
	CreateQuoteCommand,
	CreateQuotesFromJsonQuery,
	CreateRoleCommand,
	CreateShelfCommand,
	CreateTagCommand,
	CreateVoteCommand,
	DeleteVoteCommand,
	FaqDto,
	FollowUserCommand,
	FullQuoteDto,
	GetClubsWithStoryResult,
	GetFolderResult,
	GetJoinedClubsResponse,
	GetRevisionResult,
	GetSignInDataResult,
	GetThreadDetailsResult,
	GetUserNotificationsResult,
	InviteCodeDto,
	IssueInviteCodeCommand,
	JoinClubCommand,
	LeaveClubCommand,
	LockThreadCommand,
	MarkChapterAsReadCommand,
	MarkChapterAsUnreadCommand,
	QuickShelvesResult,
	QuoteDto,
	RatingApiDto,
	RemoveBookFromShelfCommand,
	RemoveBookFromShelfResult,
	ReportContentCommand,
	RoleDto,
	ShelfAddResult,
	ShelfDto,
	ShelfResult,
	SubscribeCommentsThreadCommand,
	TagDto,
	UnblockUserCommand,
	UnfollowUserCommand,
	UpdateCommentCommand,
	UpdateCommentResponse,
	UpdateFaqCommand,
	UpdateQuoteCommand,
	UpdateRoleCommand,
	UpdateRolesCommand,
	UpdateShelfCommand,
	UpdateTagCommand,
	VoteResult,
} from './types-public';
import { typedFetch } from './typed-fetch';


export const DeleteApiChaptersread = async (body: MarkChapterAsUnreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[]>("/api/chaptersread",
	"DELETE",
	body,
	headers,
	options,
);


export const DeleteApiClubjoin = async (body: LeaveClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean|string>("/api/clubjoin",
	"DELETE",
	body,
	headers,
	options,
);


export const DeleteApiComments = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/comments/${commentId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiFaqs = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/faqs?id=${id}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiInviteCodes = async (codeId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/InviteCodes/${codeId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiNotifications = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>(`/api/notifications/${id}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiQuotes = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/quotes/${id}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiRatings = async (ratingId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/ratings/${ratingId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiRoles = async (roleId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/roles?roleId=${roleId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiShelfStories = async (body: RemoveBookFromShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RemoveBookFromShelfResult>("/api/ShelfStories",
	"DELETE",
	body,
	headers,
	options,
);


export const DeleteApiShelves = async (shelfId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/shelves/${shelfId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiSubscriptionsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/subscriptions/thread?threadId=${threadId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiTags = async (tagId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/tags?tagId=${tagId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteApiUsersBlock = async (body: UnblockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/block",
	"DELETE",
	body,
	headers,
	options,
);


export const DeleteApiUsersFollow = async (body: UnfollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/follow",
	"DELETE",
	body,
	headers,
	options,
);


export const DeleteApiVotes = async (body: DeleteVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult>("/api/votes",
	"DELETE",
	body,
	headers,
	options,
);


export const GetAllQuotes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<FullQuoteDto[]>("/api/quotes",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiChaptersread = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[]>(`/api/chaptersread/${id}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiClubjoin = async (clubId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/clubjoin/${clubId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiClubsStory = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetClubsWithStoryResult[]>(`/api/clubs/story/${storyId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiClubsUser = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetJoinedClubsResponse[]>("/api/clubs/user",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiComments = async (thread: number, page: number, highlight: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{
	elements: CommentDto[];
	total: number;
	perPage: number;
	pages: number;
	page: number;
}>(`/api/comments?thread=${thread}&page=${page}&highlight=${highlight}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiCommentsMd = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>(`/api/comments/${commentId}/md`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiCommentsRevisions = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetRevisionResult[]>(`/api/comments/${commentId}/revisions`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiCommentsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetThreadDetailsResult>(`/api/CommentsThread/${threadId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiFaqs = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto[]>("/api/faqs",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiFolders = async (clubId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetFolderResult[]>(`/api/folders?clubId=${clubId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[]>("/api/InviteCodes",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiInviteCodesPaginated = async (page: number, perPage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[]>(`/api/InviteCodes/paginated?page=${page}&perPage=${perPage}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiNotifications = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserNotificationsResult[]>("/api/notifications",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiNotificationsCount = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/notifications/count",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiQuotesRandom = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto>("/api/quotes/random",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto[]>("/api/ratings",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiRoles = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto[]>("/api/roles",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiShelfStories = async (storyId: number, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfResult[]>(`/api/ShelfStories/${storyId}?page=${page}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiShelfStoriesQuick = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuickShelvesResult[]>(`/api/ShelfStories/${storyId}/quick`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiShelves = async (userName: string, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto[]>(`/api/shelves/${userName}?page=${page}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiSignin = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetSignInDataResult>(`/api/signin?name=${name}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiSubscriptionsThread = async (threadId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/subscriptions/thread?threadId=${threadId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiTags = async (page: number, perPage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags?page=${page}&perPage=${perPage}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiTagsAll = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>("/api/tags/all",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiTagsSearch = async (searchString: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags/search?searchString=${searchString}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiTagsStory = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags/story/${storyId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiTest = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/test",
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiUsersNames = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string[]|string>(`/api/users/names?name=${name}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetApiVotes = async (storyId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult>(`/api/votes/${storyId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetComment = async (commentId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<CommentDto>(`/api/comments/${commentId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetRatingById = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>(`/api/ratings/${id}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetRoleById = async (roleId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto>(`/api/roles/${roleId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetShelf = async (shelfId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>(`/api/shelves/${shelfId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetSingleFaq = async (faqId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto>(`/api/faqs/${faqId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetSingleQuote = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto>(`/api/quotes/${id}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetSingleTag = async (tagId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto>(`/api/tags/${tagId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const PatchApiComments = async (body: UpdateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<UpdateCommentResponse>("/api/comments",
	"PATCH",
	body,
	headers,
	options,
);


export const PostApiChaptersread = async (body: MarkChapterAsReadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[]>("/api/chaptersread",
	"POST",
	body,
	headers,
	options,
);


export const PostApiClubjoin = async (body: JoinClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/clubjoin",
	"POST",
	body,
	headers,
	options,
);


export const PostApiComments = async (body: CreateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/comments",
	"POST",
	body,
	headers,
	options,
);


export const PostApiCommentsThreadLock = async (body: LockThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/CommentsThread/lock",
	"POST",
	body,
	headers,
	options,
);


export const PostApiFaqs = async (body: CreateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto>("/api/faqs",
	"POST",
	body,
	headers,
	options,
);


export const PostApiFoldersAddStory = async (body: AddStoryToFolderCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<AddStoryToFolderResponse|string>("/api/folders/AddStory",
	"POST",
	body,
	headers,
	options,
);


export const PostApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto|string>("/api/InviteCodes",
	"POST",
	undefined,
	headers,
	options,
);


export const PostApiInviteCodesNoLimit = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto>("/api/InviteCodes/no-limit",
	"POST",
	undefined,
	headers,
	options,
);


export const PostApiQuotes = async (body: CreateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto>("/api/quotes",
	"POST",
	body,
	headers,
	options,
);


export const PostApiQuotesJson = async (body: CreateQuotesFromJsonQuery, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/quotes/json",
	"POST",
	body,
	headers,
	options,
);


export const PostApiRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>("/api/ratings",
	"POST",
	undefined,
	headers,
	options,
);


export const PostApiReports = async (body: ReportContentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/reports",
	"POST",
	body,
	headers,
	options,
);


export const PostApiRoles = async (body: CreateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto|string>("/api/roles",
	"POST",
	body,
	headers,
	options,
);


export const PostApiShelfStories = async (body: AddBookToShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfAddResult>("/api/ShelfStories",
	"POST",
	body,
	headers,
	options,
);


export const PostApiShelves = async (body: CreateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>("/api/shelves",
	"POST",
	body,
	headers,
	options,
);


export const PostApiSubscriptionsThread = async (body: SubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/subscriptions/thread",
	"POST",
	body,
	headers,
	options,
);


export const PostApiTags = async (body: CreateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto|string>("/api/tags",
	"POST",
	body,
	headers,
	options,
);


export const PostApiUsersBlock = async (body: BlockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/block",
	"POST",
	body,
	headers,
	options,
);


export const PostApiUsersFollow = async (body: FollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/follow",
	"POST",
	body,
	headers,
	options,
);


export const PostApiUsersRoles = async (body: UpdateRolesCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/users/roles",
	"POST",
	body,
	headers,
	options,
);


export const PostApiVotes = async (body: CreateVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult>("/api/votes",
	"POST",
	body,
	headers,
	options,
);


export const PutApiFaqs = async (body: UpdateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/faqs",
	"PUT",
	body,
	headers,
	options,
);


export const PutApiQuotes = async (body: UpdateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/quotes",
	"PUT",
	body,
	headers,
	options,
);


export const PutApiRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>("/api/ratings",
	"PUT",
	undefined,
	headers,
	options,
);


export const PutApiRoles = async (body: UpdateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/roles",
	"PUT",
	body,
	headers,
	options,
);


export const PutApiShelves = async (body: UpdateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/shelves",
	"PUT",
	body,
	headers,
	options,
);


export const PutApiTags = async (body: UpdateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>("/api/tags",
	"PUT",
	body,
	headers,
	options,
);


export const UpdateLastActive = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/useractivity",
	"HEAD",
	undefined,
	headers,
	options,
);

