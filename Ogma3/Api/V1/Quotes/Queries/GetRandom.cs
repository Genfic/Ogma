using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;

namespace Ogma3.Api.V1.Quotes.Queries
{
    public static class GetRandom
    {
        public sealed record Query : IRequest<ActionResult<QuoteDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<QuoteDto>>
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
                    .FromSqlRaw(@"
                        SELECT *
                        FROM ""Quotes""
                        OFFSET floor(random() * (
                            SELECT count(*)
                            FROM ""Quotes""
                        ))
                        LIMIT 1
                    ")
                    .AsNoTracking()
                    .ProjectTo<QuoteDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                return quote is null
                    ? new NotFoundResult()
                    : new OkObjectResult(quote);
            }
        }
    }
}