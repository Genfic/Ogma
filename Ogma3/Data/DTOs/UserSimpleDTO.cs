using Microsoft.Extensions.Configuration;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Data.DTOs
{
    public class UserSimpleDTO
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }
        public UserSimpleDTO(IConfiguration config, OgmaUser user)
        {
            UserName = user.UserName;
            Avatar = user.Avatar == null 
                ? Lorem.Gravatar(user.Email) 
                : config["cdn"] + user.Avatar;
            Title = user.Title;
        }

    }
}