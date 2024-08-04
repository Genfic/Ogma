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

export const GetApiChaptersread = async (id: number, headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<number[]>(`/api/chaptersread/${id}`, "GET", undefined, headers, options);

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

export const PostApiQuotesJson = async (body: QuoteDto[], headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/api/quotes/json", 
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

export const UpdateLastActive = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/useractivity", 
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

export const Test_GetTest = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>("/api/test", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Tags_GetAll = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>("/api/tags/all", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Tags_Search = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags/search?name=${name}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Tags_GetTags = async (page: number, perpage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags?page=${page}&perpage=${perpage}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Tags_PutTag = async (body: UpdateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ProblemDetails>("/api/tags", 
    'PUT', 
    body,
    headers,
    options,
  );

export const Tags_PostTag = async (body: CreateTagCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ProblemDetails>("/api/tags", 
    'POST', 
    body,
    headers,
    options,
  );

export const Tags_GetTag = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto>(`/api/tags/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Tags_DeleteTag = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/tags/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const Tags_GetStoryTags = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<TagDto[]>(`/api/tags/story/${id}`, 
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

export const SignIn_GetSignIn = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetSignInDataResult>(`/api/signin?name=${name}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Shelves_GetShelf = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>(`/api/shelves/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Shelves_DeleteShelf = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/shelves/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const Shelves_GetUserShelves = async (name: string, page: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto[]>(`/api/shelves/${name}?page=${page}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Shelves_PostShelf = async (body: CreateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>("/api/shelves", 
    'POST', 
    body,
    headers,
    options,
  );

export const Shelves_PutShelf = async (body: UpdateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ShelfDto>("/api/shelves", 
    'PUT', 
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

export const Reports_PostReports = async (body: ReportContentCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number|ProblemDetails>("/api/reports", 
    'POST', 
    body,
    headers,
    options,
  );

export const Ratings_GetRatings = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto[]>("/api/ratings", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Ratings_PostRating = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>("/api/ratings", 
    'POST', 
    undefined,
    headers,
    options,
  );

export const Ratings_PutRating = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto|ProblemDetails>("/api/ratings", 
    'PUT', 
    undefined,
    headers,
    options,
  );

export const Ratings_GetRating = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<RatingApiDto>(`/api/ratings/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Ratings_DeleteRating = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/ratings/${id}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const InviteCodes_GetInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[]>("/api/invitecodes", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const InviteCodes_PostInviteCode = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto>("/api/invitecodes", 
    'POST', 
    undefined,
    headers,
    options,
  );

export const InviteCodes_GetPaginatedInviteCodes = async (page: number, perpage: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto[]>(`/api/invitecodes/paginated?page=${page}&perpage=${perpage}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const InviteCodes_PostInviteCodeNoLimit = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<InviteCodeDto>("/api/invitecodes/no-limit", 
    'POST', 
    undefined,
    headers,
    options,
  );

export const InviteCodes_DeleteInviteCode = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>(`/api/invitecodes/${id}`, 
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

export const Faqs_GetFaqs = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto[]>("/api/faqs", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Faqs_PutFaq = async (body: UpdateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<ProblemDetails>("/api/faqs", 
    'PUT', 
    body,
    headers,
    options,
  );

export const Faqs_PostFaq = async (body: CreateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto>("/api/faqs", 
    'POST', 
    body,
    headers,
    options,
  );

export const Faqs_GetFaq = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<FaqDto>(`/api/faqs/${id}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Faqs_DeleteFaq = async (id: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<number|ProblemDetails>(`/api/faqs/${id}`, 
    'DELETE', 
    undefined,
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

export const ClubJoin_CheckMembershipStatus = async (club: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>(`/api/clubjoin/${club}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const ClubJoin_JoinClub = async (body: JoinClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/clubjoin", 
    'POST', 
    body,
    headers,
    options,
  );

export const ClubJoin_LeaveClub = async (body: LeaveClubCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<boolean>("/api/clubjoin", 
    'DELETE', 
    body,
    headers,
    options,
  );