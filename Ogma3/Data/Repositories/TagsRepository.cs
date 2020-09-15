using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;

namespace Ogma3.Data.Repositories
{
    public class TagsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TagsRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TagDto> GetTag(long id)
        {
            return await _context.Tags
                .Where(t => t.Id == id)
                .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<TagDto>> GetPaginated(int page = 1, int perPage = 10)
        {
            return await _context.Tags
                .Paginate(page, perPage)
                .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TagDto>> GetAll()
        {
            return await _context.Tags
                .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}