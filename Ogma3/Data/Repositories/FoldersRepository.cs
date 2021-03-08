using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.Repositories
{
    public class FoldersRepository
    {
        private readonly ApplicationDbContext _context;

        public FoldersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<FolderCard>> GetClubFolderCards(long clubId)
        {
            return await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.ParentFolderId == null)
                .Select(f => new FolderCard
                {
                    Id = f.Id,
                    Name = f.Name,
                    Slug = f.Slug,
                    Description = f.Description,
                    ClubId = f.ClubId,
                    StoriesCount = f.StoriesCount,
                    ChildFolders = f.ChildFolders.Select(cf => new FolderMinimalDto
                    {
                        Id = cf.Id,
                        Name = cf.Name,
                        Slug = cf.Slug
                    })
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FolderDto> GetFolder(long id)
        {
            return await _context.Folders
                .Where(f => f.Id == id)
                .Select(f => new FolderDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Slug = f.Slug,
                    Description = f.Description,
                    StoriesCount = f.StoriesCount,
                    AccessLevel = f.AccessLevel,
                    ChildFolders = f.ChildFolders.Select(cf => new FolderMinimal
                    {
                        Id = cf.Id,
                        ClubId = cf.ClubId,
                        Name = cf.Name,
                        Slug = cf.Slug,
                        StoriesCount = cf.StoriesCount
                    })
                })
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