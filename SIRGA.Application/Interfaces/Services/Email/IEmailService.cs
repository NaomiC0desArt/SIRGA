

using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Infrastructure;

namespace SIRGA.Application.Interfaces.Services.Email
{
    public interface IEmailService
    {
        Task<ApiResponse<bool>> SendEmailAsync(EmailRequest emailRequest);
    }
}
