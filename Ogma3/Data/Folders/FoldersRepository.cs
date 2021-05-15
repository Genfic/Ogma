using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.Folders
{
    public class FoldersRepository
    {
        private readonly ApplicationDbContext _context;

        public FoldersRepository(ApplicationDbContext context)
        {
            _context = context;
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