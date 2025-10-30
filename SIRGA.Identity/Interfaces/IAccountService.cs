using Microsoft.AspNetCore.Identity.Data;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Identity.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<ApiResponse<bool>> SignOutAsync();
        Task<ApiResponse<bool>> ConfirmEmailAsync(string userId, string token);
        Task<ApiResponse<bool>> ForgotPasswordAsync(Application.DTOs.Identity.ForgotPasswordRequest request, string origin);
        Task<ApiResponse<bool>> ResetPasswordAsync(Application.DTOs.Identity.ResetPasswordRequest request);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ApiResponse<bool>> UserExistsAsync(string email);
    }
}
