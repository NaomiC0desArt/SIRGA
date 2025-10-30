using SIRGA.Identity.Shared.Entities;

namespace SIRGA.Identity.Shared.Interfaces
{
    public interface IIdentityUrlGenerator
    {
        Task<string> GenerateEmailConfirmationUrlAsync(ApplicationUser user, string origin);

        Task<string> GeneratePasswordResetUrlAsync(ApplicationUser user, string origin);
    }
}
