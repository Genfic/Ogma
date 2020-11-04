using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.DTOs;
using Ogma3.Data.Repositories;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(FoldersController))]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly FoldersRepository _foldersRepo;

        public FoldersController(FoldersRepository foldersRepo)
        {
            _foldersRepo = foldersRepo;
        }

        // GET api/folders/5
        [HttpGet("{id}")]
        public async Task<ICollection<FolderMinimalWithParentDto>> GetChaptersRead(long id)
        {
            return await _foldersRepo.GetClubFolders(id);
        }

        [HttpGet]
        public string Ping() => "Pong";
    }
}