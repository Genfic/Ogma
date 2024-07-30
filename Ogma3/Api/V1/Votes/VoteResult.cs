namespace Ogma3.Api.V1.Votes;

public sealed record VoteResult(bool DidVote, int? Count = null);