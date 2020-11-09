using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

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

        public async Task<ICollection<FolderMinimalWithParentDto>> GetClubFolders(long clubId, long userId)
        {
            return await _context.Folders
                .TagWith($"{nameof(GetClubFolders)} — {clubId}, {userId}")
                .Where(f => f.ClubId == clubId)
                .Select(f => new FolderMinimalWithParentDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Slug = f.Slug,
                    ParentFolderId = f.ParentFolderId,
                    CanAdd = f.Club.ClubMembers.FirstOrDefault(c => c.MemberId == userId).Role <= f.AccessLevel
                })
                .AsNoTracking()
                .ToListAsync();
        }

    }
}