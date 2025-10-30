

namespace SIRGA.Application.Interfaces.Services.Email
{
    public interface IEmailTemplateGenerator
    {
        string GenerateWelcomeEmail(
            string firstName,
            string email,
            string temporaryPassword,
            string role);

        string GeneratePasswordResetEmail(string firstName, string resetUrl);

        string GenerateEmailConfirmationEmail(string firstName, string confirmationUrl);
    }
}
