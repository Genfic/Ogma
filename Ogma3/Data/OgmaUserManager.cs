using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogma3.Data.Users;

namespace Ogma3.Data
{
    public class OgmaUserManager : UserManager<OgmaUser>
    {
        public OgmaUserManager(
            IUserStore<OgmaUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<OgmaUser> passwordHasher, 
            IEnumerable<IUserValidator<OgmaUser>> userValidators, 
            IEnumerable<IPasswordValidator<OgmaUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<OgmaUser>> logger) 
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<DateTime?> GetBanDate(string name)
        {
            ThrowIfDisposed();
            var oUser = await Store.FindByNameAsync(name.ToUpper(), CancellationToken);
            return oUser.BannedUntil;
        }

        public async Task Unban(string name)
        {
            ThrowIfDisposed();
            var oUser = await Store.FindByNameAsync(name.ToUpper(), CancellationToken);
            oUser.BannedUntil = null;
            await Store.UpdateAsync(oUser, CancellationToken);
        }
    }

}