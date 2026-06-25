using JetBrains.Annotations;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.InviteCodes;

[UsedImplicitly]
public sealed class InviteCodeDto
{
	public required long Id { get; init; }
	public required string Code { get; init; }
	public required string? UsedByUserName { get; init; }
	public required string? IssuedByUserName { get; init; }
	public required DateTimeOffset IssueDate { get; init; }
	public required DateTimeOffset? UsedDate { get; init; }
	public required string? IssuedByType { get; init; }
}

[Mapper]
public static partial class InviteCodeMapper
{
	public static partial IQueryable<InviteCodeDto> ProjectToDto(this IQueryable<InviteCode> invites);

	public static partial InviteCodeDto ToDto(InviteCode ic);
}