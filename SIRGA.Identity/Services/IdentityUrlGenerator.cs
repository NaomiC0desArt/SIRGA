using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Identity.Shared.Interfaces;
using System.Text;

namespace SIRGA.Identity.Services
{
    public class IdentityUrlGenerator : IIdentityUrlGenerator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityUrlGenerator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GenerateEmailConfirmationUrlAsync(
            ApplicationUser user,
            string origin)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var baseUri = new Uri($"{origin}/Account/ConfirmEmail");

            return QueryHelpers.AddQueryString(baseUri.ToString(), new Dictionary<string, string>
            {
                { "userId", user.Id },
                { "token", encodedToken }
            });
        }

       
        public async Task<string> GeneratePasswordResetUrlAsync(
            ApplicationUser user,
            string origin)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var baseUri = new Uri($"{origin}/Account/ResetPassword");

            return QueryHelpers.AddQueryString(baseUri.ToString(), new Dictionary<string, string>
            {
                { "email", user.Email },
                { "token", encodedToken }
            });
        }
    }
}
