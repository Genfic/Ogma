using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.AuthorizationData;

namespace Ogma3.Areas.Admin.Pages
{
    [Authorize(Roles = RoleNames.Admin)]
    public class Settings : PageModel
    {
        private readonly OgmaConfig _config;

        public Settings(OgmaConfig config)
        {
            _config = config;
        }

        [BindProperty]
        public OgmaConfig Config { get; set; }

        public void OnGet()
        {
            Config = _config;
        }

        public async Task OnPostAsync()
        {
            _config.Cdn = Config.Cdn;
            _config.AdminEmail = Config.AdminEmail;
            _config.MaxInvitesPerUser = Config.MaxInvitesPerUser;
            
            await _config.PersistAsync();
        }

    }
}