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

namespace Ogma3.Api.V1.Tags.Queries;

public static class GetSingleTag
{
    public sealed record Query(long TagId) : IRequest<ActionResult<TagDto>>;

    public class Handler : IRequestHandler<Query, ActionResult<TagDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
            
        public Handler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
            
        public async Task<ActionResult<TagDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags
                .Where(t => t.Id == request.TagId)
                .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return tag is null 
                ? new NotFoundResult() 
                : new OkObjectResult(tag);
        }
    }
}