using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class FoldersRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FoldersRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ICollection<FolderCard>> GetClubFolderCards(long clubId)
        {
            return await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.ParentFolderId == null)
                .ProjectTo<FolderCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FolderDto> GetFolder(long id)
        {
            return await _context.Folders
                .Where(f => f.Id == id)
                .ProjectTo<FolderDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<FolderMinimalWithParentDto>> GetClubFolders(long clubId)
        {
            return await _context.Folders
                .Where(f => f.ClubId == clubId)
                .ProjectTo<FolderMinimalWithParentDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}