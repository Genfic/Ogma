using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class UserSimpleDTO
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }

        public static UserSimpleDTO FromUser(User user)
        {
            return new UserSimpleDTO
            {
                UserName = user.UserName,
                Avatar = user.Avatar,
                Title = user.Title
            };
        }
    }
}