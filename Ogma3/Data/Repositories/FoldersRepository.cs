using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Models;

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

        public async Task<ICollection<Folder>> GetTopLevelOfClub(long clubId)
        {
            return await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.ParentFolderId == null)
                .Include(f => f.ChildFolders)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Folder> GetFolder(long id)
        {
            return await _context.Folders
                .Where(f => f.Id == id)
                .Include(f => f.ChildFolders)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}