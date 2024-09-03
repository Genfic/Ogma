import type {
	AddBookToShelfCommand,
	AddStoryToFolderCommand,
	AddStoryToFolderResponse,
	BanUserCommand,
	BlockUserCommand,
	CommentDto,
	CommentsControllerPostData,
	CreateFaqCommand,
	CreateQuoteCommand,
	CreateQuotesFromJsonQuery,
	CreateRoleCommand,
	CreateShelfCommand,
	CreateTagCommand,
	CreateVoteCommand,
	DeleteVoteCommand,
	ErrorControllerResult,
	FaqDto,
	FollowUserCommand,
	GetClubsWithStoryResult,
	GetFolderResult,
	GetJoinedClubsResponse,
	GetPermissionsResult,
	GetRevisionResult,
	GetSignInDataResult,
	GetUserNotificationsResult,
	InviteCodeDto,
	JoinClubCommand,
	LeaveClubCommand,
	LockThreadCommand,
	MarkChapterAsReadCommand,
	MarkChapterAsUnreadCommand,
	PaginationResultOfCommentDto,
	ProblemDetails,
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
	UnbanUserCommand,
	UnblockUserCommand,
	UnfollowUserCommand,
	UnsubscribeCommentsThreadCommand,
	UpdateCommentCommand,
	UpdateFaqCommand,
	UpdateQuoteCommand,
	UpdateRoleCommand,
	UpdateRolesCommand,
	UpdateShelfCommand,
	UpdateTagCommand,
	VoteResult,
} from "./types-public";
import { typedFetch } from "./typed-fetch";

export const GetBlogpostsRssFeed = async (headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<void>("/rss/blogposts", "GET", undefined, headers, options);

export const GetChaptersRssFeed = async (storyid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>(`/rss/story/${storyid}/chapters`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetStoriesRssFeed = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/rss/stories", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiChaptersread = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[]>(`/api/chaptersread/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiChaptersread = async (body: MarkChapterAsReadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[]>("/api/chaptersread", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiChaptersread = async (body: MarkChapterAsUnreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number[]>("/api/chaptersread", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const GetApiClubjoin = async (clubid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/clubjoin/${clubid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiClubjoin = async (body: JoinClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/clubjoin", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiClubjoin = async (body: LeaveClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean|string>("/api/clubjoin", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const PostApiFaqs = async (body: CreateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto>("/api/faqs", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiFaqs = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/faqs?id=${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetApiFaqs = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto[]>("/api/faqs", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PutApiFaqs = async (body: UpdateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/faqs", 
    'PUT', 
    body,
    headers,
    options,
  );

export const GetSingleFaq = async (faqid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto>(`/api/faqs/${faqid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiInviteCodesNoLimit = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto>("/api/invitecodes/no-limit", 
    'POST', 
    undefined,
    headers,
    options,
  );

export const DeleteApiInviteCodes = async (codeid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/invitecodes/${codeid}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[]>("/api/invitecodes", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto|string>("/api/invitecodes", 
    'POST', 
    undefined,
    headers,
    options,
  );

export const GetApiInviteCodesPaginated = async (page: number, perpage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[]>(`/api/invitecodes/paginated?page=${page}&perpage=${perpage}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiNotificationsCount = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/notifications/count", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const DeleteApiNotifications = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>(`/api/notifications/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetApiNotifications = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserNotificationsResult[]>("/api/notifications", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiQuotes = async (body: CreateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto>("/api/quotes", 
    'POST', 
    body,
    headers,
    options,
  );

export const GetAllQuotes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto[]>("/api/quotes", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PutApiQuotes = async (body: UpdateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/quotes", 
    'PUT', 
    body,
    headers,
    options,
  );

export const PostApiQuotesJson = async (body: CreateQuotesFromJsonQuery, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/quotes/json", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiQuotes = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/quotes/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetSingleQuote = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto>(`/api/quotes/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiQuotesRandom = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuoteDto>("/api/quotes/random", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiRatings = async (parameters: any, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>(`/api/ratings?parameters=${parameters}`, 
    'POST', 
    undefined,
    headers,
    options,
  );

export const PutApiRatings = async (parameters: any, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>(`/api/ratings?parameters=${parameters}`, 
    'PUT', 
    undefined,
    headers,
    options,
  );

export const DeleteApiRatings = async (ratingid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/ratings/${ratingid}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto[]>("/ratings", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetRatingById = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>(`/api/ratings/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiReports = async (body: ReportContentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/reports", 
    'POST', 
    body,
    headers,
    options,
  );

export const PostApiShelfStories = async (body: AddBookToShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfAddResult>("/api/shelfstories", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiShelfStories = async (body: RemoveBookFromShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RemoveBookFromShelfResult>("/api/shelfstories", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const GetApiShelfStoriesQuick = async (storyid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<QuickShelvesResult[]>(`/api/shelfstories/${storyid}/quick`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiShelfStories = async (storyid: number, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfResult[]>(`/api/shelfstories/${storyid}?page=${page}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiShelves = async (body: CreateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>("/api/shelves", 
    'POST', 
    body,
    headers,
    options,
  );

export const PutApiShelves = async (body: UpdateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/shelves", 
    'PUT', 
    body,
    headers,
    options,
  );

export const DeleteApiShelves = async (shelfid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/shelves/${shelfid}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetShelf = async (shelfid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>(`/api/shelves/${shelfid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiShelves = async (username: string, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto[]>(`/api/shelves/${username}?page=${page}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiSignin = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetSignInDataResult>(`/api/signin?name=${name}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiTags = async (body: CreateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto|string>("/api/tags", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiTags = async (tagid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/tags?tagid=${tagid}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const GetApiTags = async (page: number, perpage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags?page=${page}&perpage=${perpage}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PutApiTags = async (body: UpdateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>("/api/tags", 
    'PUT', 
    body,
    headers,
    options,
  );

export const GetApiTagsAll = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>("/api/tags/all", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetSingleTag = async (tagid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto>(`/api/tags/${tagid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiTagsStory = async (storyid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags/story/${storyid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiTagsSearch = async (searchstring: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags/search?searchstring=${searchstring}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const GetApiTest = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/test", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const UpdateLastActive = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/useractivity", 
    'HEAD', 
    undefined,
    headers,
    options,
  );

export const PostApiUsersBlock = async (body: BlockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/block", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiUsersBlock = async (body: UnblockUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/block", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const GetApiUsersNames = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string[]|string>(`/api/users/names?name=${name}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const PostApiUsersFollow = async (body: FollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/follow", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiUsersFollow = async (body: UnfollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/users/follow", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const PostApiUsersRoles = async (body: UpdateRolesCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/users/roles", 
    'POST', 
    body,
    headers,
    options,
  );

export const PostApiVotes = async (body: CreateVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult>("/api/votes", 
    'POST', 
    body,
    headers,
    options,
  );

export const DeleteApiVotes = async (body: DeleteVoteCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult>("/api/votes", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const GetApiVotes = async (storyid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<VoteResult>(`/api/votes/${storyid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Chapter_FirstChapter = async (sid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>(`/story/${sid}/chapter/first`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Chapter_LastChapter = async (sid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>(`/story/${sid}/chapter/last`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Error_OnGet = async (code: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ErrorControllerResult>(`/api/error?code=${code}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Subscriptions_IsSubscribedToThread = async (threadid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/subscriptions/thread?threadid=${threadid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Subscriptions_SubscribeThread = async (body: SubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/subscriptions/thread", 
    'POST', 
    body,
    headers,
    options,
  );

export const Subscriptions_UnsubscribeThread = async (body: UnsubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/subscriptions/thread", 
    'DELETE', 
    body,
    headers,
    options,
  );

export const Roles_GetRoles = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto[]>("/api/roles", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Roles_PutRole = async (body: UpdateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto>("/api/roles", 
    'PUT', 
    body,
    headers,
    options,
  );

export const Roles_PostRole = async (body: CreateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto>("/api/roles", 
    'POST', 
    body,
    headers,
    options,
  );

export const Roles_GetRole = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RoleDto>(`/api/roles/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Roles_DeleteRole = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ProblemDetails>(`/api/roles/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const Folders_GetFoldersOfClub = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetFolderResult[]>(`/api/folders/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Folders_AddStory = async (body: AddStoryToFolderCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<AddStoryToFolderResponse>("/api/folders/add-story", 
    'POST', 
    body,
    headers,
    options,
  );

export const CommentsThread_GetPermissions = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetPermissionsResult>(`/api/commentsthread/permissions/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const CommentsThread_GetLockStatus = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/commentsthread/lock/status/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const CommentsThread_LockThread = async (body: LockThreadCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean|ProblemDetails>("/api/commentsthread/lock", 
    'POST', 
    body,
    headers,
    options,
  );

export const Comments_GetComments = async (thread: number, page: number, highlight: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<PaginationResultOfCommentDto>(`/api/comments?thread=${thread}&page=${page}&highlight=${highlight}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Comments_PostComments = async (body: CommentsControllerPostData, headers?: HeadersInit, options?: RequestInit) => await typedFetch<CommentDto>("/api/comments", 
    'POST', 
    body,
    headers,
    options,
  );

export const Comments_PutComment = async (body: UpdateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<CommentDto>("/api/comments", 
    'PATCH', 
    body,
    headers,
    options,
  );

export const Comments_GetComment = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<CommentDto>(`/api/comments/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Comments_DeleteComment = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/comments/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const Comments_GetRevisions = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetRevisionResult[]>(`/api/comments/revisions/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Comments_GetMarkdown = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>(`/api/comments/md?id=${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Clubs_GetUserClubs = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetJoinedClubsResponse[]>("/api/clubs/user", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Clubs_GetClubsWithStory = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetClubsWithStoryResult[]>(`/api/clubs/story/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Clubs_BanUser = async (body: BanUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/clubs/user/ban", 
    'POST', 
    body,
    headers,
    options,
  );

export const Clubs_UnbanUser = async (body: UnbanUserCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/clubs/user/ban", 
    'DELETE', 
    body,
    headers,
    options,
  );