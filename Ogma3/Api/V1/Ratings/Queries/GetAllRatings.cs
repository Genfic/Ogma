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
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Ratings.Queries;

public static class GetAllRatings
{
	public sealed record Query : IRequest<ActionResult<List<RatingApiDto>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<RatingApiDto>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async ValueTask<ActionResult<List<RatingApiDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var ratings = await _context.Ratings
				.OrderBy(r => r.Order)
				.ProjectTo<RatingApiDto>(_mapper.ConfigurationProvider)
				.ToListAsync(cancellationToken);

			return Ok(ratings);
		}
	}
}