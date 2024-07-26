using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Reports;
using Ogma3.Pages.Shared;
using Ogma3.Services;

namespace Ogma3.Areas.Admin.Pages;

public class Reports : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly CommentRedirector _redirector;
	private readonly IMapper _mapper;
	private const int PerPage = 50;

	public Reports(ApplicationDbContext context, CommentRedirector redirector, IMapper mapper)
	{
		_context = context;
		_redirector = redirector;
		_mapper = mapper;
	}

	public required List<ReportDto> ReportsList { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task OnGetAsync([FromQuery] int page = 1)
	{
		ReportsList = await _context.Reports
			.OrderByDescending(r => r.ReportDate)
			.Paginate(page, PerPage)
			.ProjectTo<ReportDto>(_mapper.ConfigurationProvider)
			.ToListAsync();
		var count = await _context.Reports.CountAsync();

		Pagination = new Pagination
		{
			CurrentPage = page,
			ItemCount = count,
			PerPage = PerPage
		};
	}

	/// <summary>
	/// The comment system here is so incredibly complex it needs its own named handler lol
	/// Its purpose is to redirect to the content the given comment is attached to, and to scroll to said comment
	/// </summary>
	/// <param name="id">ID of the comment</param>
	/// <returns></returns>
	public async Task<ActionResult> OnGetComment(long id)
	{
		var redirect = await _redirector.RedirectToComment(id);

		if (redirect is null) return NotFound();

		return RedirectToPage(redirect.Url, null, redirect.Params, redirect.Fragment);
	}
}