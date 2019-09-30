using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ogma3.Data
{
    public class OgmaUserManager : UserManager<User>
    {
        public OgmaUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<string> GetTitleAsync(IdentityUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var oUser = await Store.FindByIdAsync(user.Id, CancellationToken);
            return oUser.Title;
        }

        public async Task<string> GetBioAsync(IdentityUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var oUser = await Store.FindByIdAsync(user.Id, CancellationToken);
            return oUser.Bio;
        }

        public async Task<string> GetUserAvatarAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var oUser = await Store.FindByIdAsync(user.Id, CancellationToken);
            return oUser.Avatar;
        }
    }

}