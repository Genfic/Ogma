using System;

namespace Ogma3.Pages.Shared;

public record CommentsThreadDto(long Id, string Type, DateTime? LockDate);