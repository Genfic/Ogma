using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Quotes.Queries;

public static class GetOne
{
	public sealed record Query(long Id) : IRequest<ActionResult<QuoteDto>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<QuoteDto>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ActionResult<QuoteDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var quote = await _context.Quotes
				.Where(q => q.Id == request.Id)
				.ProjectTo<QuoteDto>(_mapper.ConfigurationProvider)
				.FirstOrDefaultAsync(cancellationToken);

			return quote is null
				? NotFound()
				: Ok(quote);
		}
	}
}