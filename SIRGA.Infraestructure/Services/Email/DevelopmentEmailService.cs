using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Infrastructure;
using SIRGA.Application.Interfaces.Services.Email;


namespace SIRGA.Infraestructure.Services.Email
{
    public class DevelopmentEmailService : IEmailService
    {
        private readonly ILogger<DevelopmentEmailService> _logger;

        public DevelopmentEmailService(ILogger<DevelopmentEmailService> logger)
        {
            _logger = logger;
        }

        public Task<ApiResponse<bool>> SendEmailAsync(EmailRequest emailRequest)
        {
            _logger.LogWarning("====================================");
            _logger.LogWarning("EMAIL SIMULADO (NO SE ENVIÓ REAL)");
            _logger.LogWarning($"Para: {emailRequest.To}");
            _logger.LogWarning($"Asunto: {emailRequest.Subject}");
            _logger.LogWarning($"Cuerpo:\n{emailRequest.Body}");
            _logger.LogWarning("====================================");

            return Task.FromResult(
                ApiResponse<bool>.SuccessResponse(
                    true,
                    "Email simulado (solo logs)"
                )
            );
        }
    }
}
