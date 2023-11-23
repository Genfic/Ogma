using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.Roles;

namespace Ogma3.Areas.Admin.Pages;

public class Users : PageModel
{
	private readonly ApplicationDbContext _context;
	public Users(ApplicationDbContext context) => _context = context;

	public UserDetailsDto? OgmaUser { get; private set; }
	public List<OgmaRole> Roles { get; private set; } = null!;

	public async Task<ActionResult> OnGet([FromQuery] string? name)
	{
		if (string.IsNullOrEmpty(name)) return Page();

		var query = _context.Users.AsQueryable();

		query = name.StartsWith("id:", StringComparison.InvariantCultureIgnoreCase)
			? query.Where(u => u.Id == int.Parse(name.Replace("id:", "", StringComparison.InvariantCultureIgnoreCase)))
			: query.Where(u => u.NormalizedUserName == name.ToUpper());

		OgmaUser = await query.Select(u => new UserDetailsDto
			{
				Id = u.Id,
				Name = u.UserName ?? "",
				Email = u.Email ?? "",
				Title = u.Title,
				Avatar = u.Avatar,
				Bio = u.Bio,
				RoleNames = u.Roles.Select(r => r.Name ?? ""),
				RegistrationDate = u.RegistrationDate,
				LastActive = u.LastActive,
				StoriesCount = u.Stories.Count,
				BlogpostsCount = u.Blogposts.Count,
				Infractions = u.Infractions
					.OrderByDescending(i => i.Type)
					.ThenByDescending(i => i.ActiveUntil)
					.Select(i => new InfractionDto
					{
						Id = i.Id,
						Reason = i.Reason,
						Type = i.Type,
						ActiveUntil = i.ActiveUntil,
						IssueDate = i.IssueDate,
						IssuedBy = i.IssuedBy.UserName ?? "",
						RemovedAt = i.RemovedAt,
						RemovedBy = i.RemovedBy != null ? i.RemovedBy.UserName : null
					})
					.ToList()
			})
			.FirstOrDefaultAsync();
		if (OgmaUser is null) return NotFound();

		Roles = await _context.Roles.ToListAsync();

		return Page();
	}

	public sealed record UserDetailsDto
	{
		public long Id { get; init; }
		public string Name { get; init; } = null!;
		public string Email { get; init; } = null!;
		public string? Title { get; init; }
		public string? Bio { get; init; }
		public string? Avatar { get; init; }
		public DateTime RegistrationDate { get; init; }
		public DateTime LastActive { get; init; }
		public int StoriesCount { get; init; }
		public int BlogpostsCount { get; init; }
		public IEnumerable<string> RoleNames { get; init; } = null!;
		public ICollection<InfractionDto> Infractions { get; init; } = null!;
	}

	public sealed record InfractionDto
	{
		public long Id { get; init; }
		public DateTime IssueDate { get; init; }
		public DateTime ActiveUntil { get; init; }
		public DateTime? RemovedAt { get; init; }
		public string Reason { get; init; } = null!;
		public InfractionType Type { get; init; }
		public string IssuedBy { get; init; } = null!;
		public string? RemovedBy { get; init; }
	}
}