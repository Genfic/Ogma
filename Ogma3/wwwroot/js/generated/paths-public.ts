import type {
	AddStoryToFolderCommand,
	BanUserCommand,
	BlockUserCommand,
	CommentsControllerPostData,
	CreateFaqCommand,
	CreateInfractionCommand,
	CreateQuoteCommand,
	CreateRoleCommand,
	CreateShelfCommand,
	CreateTagCommand,
	CreateVoteCommand,
	DeleteQuoteCommand,
	DeleteVoteCommand,
	FollowUserCommand,
	JoinClubCommand,
	LeaveClubCommand,
	LockThreadCommand,
	MarkChapterAsReadCommand,
	MarkChapterAsUnreadCommand,
	ReportContentCommand,
	SubscribeCommentsThreadCommand,
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
} from "./types-public";

export const Chapter_FirstChapter = async (sid: number, headers?: HeadersInit, options?: RequestInit) =>
	await fetch(`/story/${sid}/chapter/first`, {
		method: "GET",
		headers: {
			"Content-Type": "application/json",
			...headers,
		},
		...options,
	});

export const Chapter_LastChapter = async (sid: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/story/${sid}/chapter/last`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Error_OnGet = async (code: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/error?code=${code}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Votes_GetVotes = async (storyid: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/votes/${storyid}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Votes_PostVote = async (body: CreateVoteCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/votes", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Votes_DeleteVote = async (body: DeleteVoteCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/votes", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Users_BlockUser = async (body: BlockUserCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/users/block", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Users_UnblockUser = async (body: UnblockUserCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/users/block", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Users_FollowUser = async (body: FollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/users/follow", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Users_UnfollowUser = async (body: UnfollowUserCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/users/follow", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Users_ManageRoles = async (body: UpdateRolesCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/users/roles", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Users_FindNames = async (name: string, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/users/names?name=${name}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const UserActivity_UpdateLastActive = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/useractivity", { 
    method: 'HEAD', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Tags_GetAll = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/tags/all", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Tags_Search = async (name: string, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/tags/search?name=${name}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Tags_GetTags = async (page: number, perpage: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/tags?page=${page}&perpage=${perpage}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Tags_PutTag = async (body: UpdateTagCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/tags", { 
    method: 'PUT', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Tags_PostTag = async (body: CreateTagCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/tags", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Tags_GetTag = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/tags/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Tags_DeleteTag = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/tags/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Tags_GetStoryTags = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/tags/story/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Subscriptions_IsSubscribedToThread = async (threadid: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/subscriptions/thread?threadid=${threadid}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Subscriptions_SubscribeThread = async (body: SubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/subscriptions/thread", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Subscriptions_UnsubscribeThread = async (body: UnsubscribeCommentsThreadCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/subscriptions/thread", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const SignIn_GetSignIn = async (name: string, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/signin?name=${name}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Shelves_GetShelf = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelves/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Shelves_DeleteShelf = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelves/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Shelves_GetUserShelves = async (name: string, page: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelves/${name}?page=${page}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Shelves_PostShelf = async (body: CreateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/shelves", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Shelves_PutShelf = async (body: UpdateShelfCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/shelves", { 
    method: 'PUT', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const ShelfStories_GetUserQuickShelves = async (storyid: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelfstories/${storyid}/quick`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const ShelfStories_GetUserShelvesPaginated = async (storyid: number, page: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelfstories/${storyid}?page=${page}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const ShelfStories_AddToShelf = async (shelfid: number, storyid: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelfstories/${shelfid}/${storyid}`, { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const ShelfStories_RemoveFromShelf = async (shelfid: number, storyid: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/shelfstories/${shelfid}/${storyid}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Roles_GetRoles = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/roles", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Roles_PutRole = async (body: UpdateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/roles", { 
    method: 'PUT', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Roles_PostRole = async (body: CreateRoleCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/roles", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Roles_GetRole = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/roles/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Roles_DeleteRole = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/roles/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Reports_PostReports = async (body: ReportContentCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/reports", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Ratings_GetRatings = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/ratings", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Ratings_PostRating = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/ratings", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Ratings_PutRating = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/ratings", { 
    method: 'PUT', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Ratings_GetRating = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/ratings/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Ratings_DeleteRating = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/ratings/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Quotes_GetQuotes = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/quotes", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Quotes_PostQuote = async (body: CreateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/quotes", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Quotes_PutQuote = async (body: UpdateQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/quotes", { 
    method: 'PUT', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Quotes_DeleteQuote = async (body: DeleteQuoteCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/quotes", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Quotes_GetQuote = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/quotes/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Quotes_GetRandomQuote = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/quotes/random", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Quotes_PostJson = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/quotes/json", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Notifications_GetUserNotifications = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/notifications", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Notifications_CountUserNotifications = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/notifications/count", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Notifications_DeleteNotification = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/notifications/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const InviteCodes_GetInviteCodes = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/invitecodes", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const InviteCodes_PostInviteCode = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/invitecodes", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const InviteCodes_GetPaginatedInviteCodes = async (page: number, perpage: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/invitecodes/paginated?page=${page}&perpage=${perpage}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const InviteCodes_PostInviteCodeNoLimit = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/invitecodes/no-limit", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const InviteCodes_DeleteInviteCode = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/invitecodes/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Infractions_GetInfractions = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/infractions?id=${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Infractions_AddInfraction = async (body: CreateInfractionCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/infractions", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Infractions_GetInfractionDetails = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/infractions/details?id=${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Infractions_DeactivateInfraction = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/infractions/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Folders_GetFoldersOfClub = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/folders/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Folders_AddStory = async (body: AddStoryToFolderCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/folders/add-story", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Faqs_GetFaqs = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/faqs", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Faqs_PutFaq = async (body: UpdateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/faqs", { 
    method: 'PUT', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Faqs_PostFaq = async (body: CreateFaqCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/faqs", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Faqs_GetFaq = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/faqs/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Faqs_DeleteFaq = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/faqs/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const CommentsThread_GetPermissions = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/commentsthread/permissions/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const CommentsThread_GetLockStatus = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/commentsthread/lock/status/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const CommentsThread_LockThread = async (body: LockThreadCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/commentsthread/lock", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Comments_GetComments = async (thread: number, page: number, highlight: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/comments?thread=${thread}&page=${page}&highlight=${highlight}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Comments_PostComments = async (body: CommentsControllerPostData, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/comments", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Comments_PutComment = async (body: UpdateCommentCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/comments", { 
    method: 'PATCH', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Comments_GetComment = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/comments/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Comments_DeleteComment = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/comments/${id}`, { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Comments_GetRevisions = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/comments/revisions/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Comments_GetMarkdown = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/comments/md?id=${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Clubs_GetUserClubs = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/clubs/user", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Clubs_GetClubsWithStory = async (id: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/clubs/story/${id}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Clubs_BanUser = async (body: BanUserCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/clubs/user/ban", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const Clubs_UnbanUser = async (body: UnbanUserCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/clubs/user/ban", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const ClubJoin_CheckMembershipStatus = async (club: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/clubjoin/${club}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const ClubJoin_JoinClub = async (body: JoinClubCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/clubjoin", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const ClubJoin_LeaveClub = async (body: LeaveClubCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/clubjoin", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const ChaptersRead_GetChaptersRead = async (story: number, headers?: HeadersInit, options?: RequestInit) => await fetch (`/api/chaptersread/${story}`, { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const ChaptersRead_PostChaptersRead = async (body: MarkChapterAsReadCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/chaptersread", { 
    method: 'POST', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });

export const ChaptersRead_DeleteChaptersRead = async (body: MarkChapterAsUnreadCommand, headers?: HeadersInit, options?: RequestInit) => await fetch ("/api/chaptersread", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    body: JSON.stringify(body),
    ...options 
  });