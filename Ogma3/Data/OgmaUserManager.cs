using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogma3.Data.Models;

namespace Ogma3.Data
{
    public class OgmaUserManager : UserManager<User>
    {
        public OgmaUserManager(
            IUserStore<User> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<User> passwordHasher, 
            IEnumerable<IUserValidator<User>> userValidators, 
            IEnumerable<IPasswordValidator<User>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<User>> logger) 
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<string> GetTitleAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var oUser = await Store.FindByIdAsync(user.Id.ToString(), CancellationToken);
            return oUser.Title;
        }

        public async Task<string> GetBioAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var oUser = await Store.FindByIdAsync(user.Id.ToString(), CancellationToken);
            return oUser.Bio;
        }

        public async Task<string> GetAvatarAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var oUser = await Store.FindByIdAsync(user.Id.ToString(), CancellationToken);
            return oUser.Avatar;
        }
    }

}