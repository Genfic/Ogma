using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Data.DTOs
{
    public class UserProfileDto : ProfileBar
    {
        public string Bio { get; set; }
        public long CommentsThreadId { get; set; }
    }
}