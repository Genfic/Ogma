using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.InviteCodes;

public class InviteCodeDto
{
	public required long Id { get; init; }
	public required string Code { get; init; } = null!;
	public required string NormalizedCode { get; init; } = null!;
	public required string? UsedByUserName { get; init; }
	public required string IssuedByUserName { get; init; } = null!;
	public required DateTime IssueDate { get; init; }
	public required DateTime? UsedDate { get; init; }
}

[Mapper]
public static partial class InviteCodeMapper
{
	public static partial IQueryable<InviteCodeDto> ProjectToDto(this IQueryable<InviteCode> invites);

	public static partial InviteCodeDto ToDto(InviteCode ic);
}