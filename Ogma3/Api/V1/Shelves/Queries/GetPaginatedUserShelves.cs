using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves.Queries;

public static class GetPaginatedUserShelves
{
	public sealed record Query(string UserName, int Page) : IRequest<ActionResult<List<ShelfDto>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<ShelfDto>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly OgmaConfig _config;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IMapper mapper, IUserService userService, OgmaConfig config)
		{
			_context = context;
			_mapper = mapper;
			_config = config;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<List<ShelfDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var (userName, page) = request;

			var shelves = await _context.Shelves
				.Where(s => s.Owner.NormalizedUserName == userName.Normalize().ToUpperInvariant())
				.Where(s => s.OwnerId == _uid || s.IsPublic)
				.Paginate(page, _config.ShelvesPerPage)
				.ProjectTo<ShelfDto>(_mapper.ConfigurationProvider)
				.ToListAsync(cancellationToken);

			return Ok(shelves);
		}
	}
}