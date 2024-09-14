using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ogma3.Data.Users;

namespace Ogma3.Data;

public sealed class OgmaUserManager
(
	IUserStore<OgmaUser> store,
	IOptions<IdentityOptions> optionsAccessor,
	IPasswordHasher<OgmaUser> passwordHasher,
	IEnumerable<IUserValidator<OgmaUser>> userValidators,
	IEnumerable<IPasswordValidator<OgmaUser>> passwordValidators,
	ILookupNormalizer keyNormalizer,
	IdentityErrorDescriber errors,
	IServiceProvider services,
	ILogger<OgmaUserManager> logger)
	: UserManager<OgmaUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger);