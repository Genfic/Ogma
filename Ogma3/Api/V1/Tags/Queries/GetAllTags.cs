using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Tags.Queries;

public static class GetAllTags
{
	public sealed record Query : IRequest<ActionResult<List<TagDto>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<TagDto>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async ValueTask<ActionResult<List<TagDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var tags = await _context.Tags
				.OrderBy(t => t.Id)
				.ProjectTo<TagDto>(_mapper.ConfigurationProvider)
				.ToListAsync(cancellationToken);

			return Ok(tags);
		}
	}
}