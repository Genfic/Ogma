using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using reCAPTCHA.AspNetCore;
using Serilog;
using Utils;

namespace Ogma3.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<OgmaUser> _signInManager;
        private readonly UserManager<OgmaUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IRecaptchaService _reCaptcha;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<OgmaUser> userManager,
            SignInManager<OgmaUser> signInManager,
            IEmailSender emailSender,
            IRecaptchaService reCaptcha,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _reCaptcha = reCaptcha;
            _context = context;
        }

        [BindProperty] public InputModel Input { get; set; }

        [Required(ErrorMessage = "ReCaptcha is required")]
        [BindProperty(Name = "g-recaptcha-response")]
        public string ReCaptchaResponse { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            public string Name { get; set; }
            public string Email { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Invite code")]
            public string? InviteCode { get; set; }
        }
        
        public class InputModelValidation : AbstractValidator<InputModel>
        {
            public InputModelValidation()
            {
                RuleFor(im => im.Name)
                    .NotEmpty()
                    .MinimumLength(CTConfig.CUser.MinNameLength)
                    .MaximumLength(CTConfig.CUser.MaxNameLength);
                RuleFor(im => im.Email)
                    .NotEmpty()
                    .EmailAddress();
                RuleFor(im => im.Password)
                    .NotEmpty()
                    .MinimumLength(CTConfig.CUser.MinPassLength)
                    .MaximumLength(CTConfig.CUser.MaxPassLength);
                RuleFor(im => im.ConfirmPassword)
                    .Equal(im => im.Password);
                RuleFor(im => im.InviteCode)
                    .NotEmpty()
                    .Length(13);
            }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid) return Page();

            // Check ReCaptcha
            var reResponse = await _reCaptcha.Validate(ReCaptchaResponse);
            if (!reResponse.success)
            {
                ModelState.TryAddModelError("ReCaptcha", "Incorrect ReCaptcha");
                return Page();
            }

            // Check if invite code is correct
            var inviteCode = await _context.InviteCodes
                .FirstOrDefaultAsync(ic => ic.NormalizedCode == Input.InviteCode.ToUpper());
            if (inviteCode is null)
            {
                ModelState.TryAddModelError("InviteCode", "Incorrect invite code");
                return Page();
            }

            if (inviteCode.UsedDate is not null)
            {
                ModelState.TryAddModelError("InviteCode", "This invite code has been used");
                return Page();
            }

            // Generate Gravatar
            var avatar = Gravatar.Generate(Input.Email, new Gravatar.Options
            {
                Default = Gravatar.AvatarGenMethods.Identicon, 
                Rating = Gravatar.Ratings.G
            });
            
            // Create user
            var user = new OgmaUser
            {
                UserName = Input.Name,
                Email = Input.Email,
                Avatar = avatar
            };
            var result = await _userManager.CreateAsync(user, Input.Password);

            // If everything went fine...
            if (result.Succeeded)
            {
                Log.Information("User {Name} created an account!", Input.Name);

                // Modify invite code
                inviteCode.UsedBy = user;
                inviteCode.UsedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                // Send confirmation code
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    null,
                    new { area = "Identity", userName = user.UserName, code },
                    Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.\n\nAlternatively, go to <pre>/confirm-email</pre> and enter the code <pre>{code}</pre>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                }

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}