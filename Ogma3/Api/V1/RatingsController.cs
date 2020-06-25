using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(RatingsController))]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<RatingApiDTO>> GetRatings()
        {
            var list = await _context.Ratings.ToListAsync();
            return list.Select(RatingApiDTO.FronRating);
        }
    }
}
