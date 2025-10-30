using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Infrastructure;
using SIRGA.Infraestructure.Settings;

using SIRGA.Application.Interfaces.Services.Email;

namespace SIRGA.Infraestructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var email = BuildEmailMessage(emailRequest);

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                // Log del intento
                _logger.LogInformation($"Intentando enviar email a {emailRequest.To}");

                // Conectar
                await smtp.ConnectAsync(
                    _emailSettings.SmtpHost,
                    _emailSettings.SmtpPort,
                    SecureSocketOptions.StartTls
                );

                _logger.LogInformation("Conectado al servidor SMTP");

                // Autenticar
                await smtp.AuthenticateAsync(
                    _emailSettings.SmtpUser,
                    _emailSettings.SmtpPass
                );

                _logger.LogInformation("Autenticación exitosa");

                // Enviar
                var result = await smtp.SendAsync(email);

                _logger.LogInformation($"Email enviado exitosamente a {emailRequest.To}. Response: {result}");

                // Desconectar
                await smtp.DisconnectAsync(true);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Email enviado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando email a {emailRequest.To}");

                // Retornar error detallado
                return ApiResponse<bool>.ErrorResponse(
                    "Error al enviar el email",
                    new List<string>
                    {
                        ex.Message,
                        ex.InnerException?.Message ?? ""
                    }
                );
            }
        }

        private MimeMessage BuildEmailMessage(EmailRequest emailRequest)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(
                    emailRequest.From ?? _emailSettings.EmailFrom
                ),
                Subject = emailRequest.Subject
            };

            // From
            email.From.Add(new MailboxAddress(
                _emailSettings.DisplayName,
                _emailSettings.EmailFrom
            ));

            // To
            email.To.Add(MailboxAddress.Parse(emailRequest.To));

            // Body (HTML)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailRequest.Body
            };

            email.Body = bodyBuilder.ToMessageBody();

            return email;
        }
    }
}
