using Ogma3.Data.Ratings;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blacklists;

public class BlacklistedRating
{
	public OgmaUser User { get; init; }
	public long UserId { get; init; }
	public Rating Rating { get; init; }
	public long RatingId { get; init; }
}