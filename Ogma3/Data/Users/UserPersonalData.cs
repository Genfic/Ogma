using System;
using System.Linq;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Users;

public class UserPersonalData
{
	public DateTimeOffset GenerationTime => DateTimeOffset.Now;
	public required long Id { get; init; }
	public required string UserName { get; init; }
	public required string Email { get; init; }
	public required bool EmailConfirmed { get; init; }
	public required bool TwoFactorEnabled { get; init; }
	public required string? Title { get; init; }
	public required string? Bio { get; init; }
	public required string[] Links { get; init; } = [];
	public required string Avatar { get; init; }
	public required DateTime RegistrationDate { get; init; }
	public required DateTime LastActive { get; init; }
}

[Mapper]
public static partial class UserPersonalDataMapper
{
	public static partial IQueryable<UserPersonalData> ProjectToDto(this IQueryable<OgmaUser> q);
}