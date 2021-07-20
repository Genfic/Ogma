using System.Collections.Generic;
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
    public static class GetAll
    {
        public sealed record Query : IRequest<ActionResult<List<QuoteDto>>>;

        public class Handler : IRequestHandler<Query, ActionResult<List<QuoteDto>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public Handler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ActionResult<List<QuoteDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quotes = await _context.Quotes
                    .ProjectTo<QuoteDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                return new OkObjectResult(quotes);
            }
        }
    }
}