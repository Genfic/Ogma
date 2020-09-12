using System.Collections.Generic;
using System.Linq;
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

        public RoleDTO TopRole { get; set; }
        public UserSimpleDTO(IConfiguration config, OgmaUser user)
        {
            UserName = user.UserName;
            
            Avatar = user.Avatar == null 
                ? Lorem.Gravatar(user.Email) 
                : config["cdn"] + user.Avatar;
            
            Title = user.Title;

            var topRole = user.Roles
                .Where(r => r.Order.HasValue)
                .OrderBy(r => r.Order)
                .FirstOrDefault();
            
            TopRole = topRole == null ? null : new RoleDTO
            {
                Id = topRole.Id,
                Name = topRole.Name,
                Color = topRole.Color,
                IsStaff = topRole.IsStaff
            };
        }
        public UserSimpleDTO(OgmaUser user)
        {
            UserName = user.UserName;

            Avatar = user.Avatar;
            
            Title = user.Title;

            var topRole = user.Roles
                .Where(r => r.Order.HasValue)
                .OrderBy(r => r.Order)
                .FirstOrDefault();
            
            TopRole = topRole == null ? null : new RoleDTO
            {
                Id = topRole.Id,
                Name = topRole.Name,
                Color = topRole.Color,
                IsStaff = topRole.IsStaff
            };
        }

    }
}