using System;
using System.Collections.Generic;
using Ogma3.Data.DTOs;

namespace Ogma3.Pages.Shared
{
    public class ProfileBar
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastActive { get; set; }

        public IEnumerable<RoleDto> Roles { get; set; }

        public int StoriesCount { get; set; }
        public int BlogpostsCount { get; set; }
    }
}