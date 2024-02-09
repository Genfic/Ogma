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

public static class GetRatingById
{
	public sealed record Query(long Id) : IRequest<ActionResult<RatingApiDto>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<RatingApiDto>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async ValueTask<ActionResult<RatingApiDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var ratings = await _context.Ratings
				.Where(r => r.Id == request.Id)
				.ProjectTo<RatingApiDto>(_mapper.ConfigurationProvider)
				.ToListAsync(cancellationToken);

			return Ok(ratings);
		}
	}
}