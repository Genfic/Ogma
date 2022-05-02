using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Tags.Queries;

public static class GetStoryTags
{
	public sealed record Query(long StoryId) : IRequest<ActionResult<List<TagDto>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<TagDto>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ActionResult<List<TagDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var tags = await _context.StoryTags
				.Where(st => st.StoryId == request.StoryId)
				.Select(st => st.Tag)
				.ProjectTo<TagDto>(_mapper.ConfigurationProvider)
				.AsNoTracking()
				.ToListAsync(cancellationToken);

			return tags is { Count: > 0 }
				? Ok(tags)
				: NotFound();
		}
	}
}