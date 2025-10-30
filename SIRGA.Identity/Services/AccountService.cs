using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SIRGA.Identity.Interfaces;
using SIRGA.Identity.Shared.Entities;
using System.Text;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Identity;
using SIRGA.Application.DTOs.Infrastructure;
using SIRGA.Application.Interfaces.Services.Email;
using SIRGA.Identity.Shared.Interfaces;

namespace SIRGA.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IIdentityUrlGenerator _urlGenerator;
        private readonly IEmailTemplateGenerator _templateGenerator;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IJwtService jwtService,
             IIdentityUrlGenerator urlGenerator,
             IEmailTemplateGenerator templateGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jwtService = jwtService;
            _urlGenerator = urlGenerator;
            _templateGenerator = templateGenerator;
        }

        /// Login de usuarios
       
        public async Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return ApiResponse<AuthenticationResponse>.ErrorResponse(
                        $"No hay cuentas registradas con el correo {request.Email}"
                    );
                }

                if (!user.IsActive)
                {
                    return ApiResponse<AuthenticationResponse>.ErrorResponse(
                        "Su cuenta ha sido desactivada. Contacte al administrador."
                    );
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    request.Password,
                    isPersistent: request.RememberMe,
                    lockoutOnFailure: false
                );

                if (!result.Succeeded)
                {
                    return ApiResponse<AuthenticationResponse>.ErrorResponse(
                        "Email o contraseña incorrectos"
                    );
                }

                // Verificar email confirmado
                // if (!user.EmailConfirmed)
                // {
                //     return ApiResponse<AuthenticationResponse>.ErrorResponse(
                //         "Debes confirmar tu email antes de iniciar sesión"
                //     );
                // }
                user.LastLogin = DateTimeOffset.Now;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);

                var jwtToken = await _jwtService.GenerateJwtTokenAsync(user, roles.ToList());

                var response = new AuthenticationResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    MustCompleteProfile = user.MustCompleteProfile,
                    JWToken = jwtToken

                };

                return ApiResponse<AuthenticationResponse>.SuccessResponse(
                    response,
                    "Login exitoso"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthenticationResponse>.ErrorResponse(
                    "Error al iniciar sesión",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Usuario no encontrado"
                    );
                }

                // Decodificar el token
                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(token)
                );

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al confirmar el email",
                        errors
                    );
                }

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    $"La cuenta con el correo {user.Email} ha sido confirmada"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al confirmar el email",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(
            Application.DTOs.Identity.ForgotPasswordRequest request,
            string origin)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                  
                    return ApiResponse<bool>.SuccessResponse(
                        true,
                        "Si el correo existe, recibirás instrucciones para restablecer tu contraseña"
                    );
                }

                
                var resetUrl = await _urlGenerator.GeneratePasswordResetUrlAsync(user, origin);

                
                var emailBody = _templateGenerator.GeneratePasswordResetEmail(
                    user.FirstName,
                    resetUrl
                );

                
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Restablecer Contraseña - SIRGA",
                    Body = emailBody
                });

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Se ha enviado un email con instrucciones para restablecer tu contraseña"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al procesar la solicitud",
                    new List<string> { ex.Message }
                );
            }
        }

        
        public async Task<ApiResponse<bool>> ResetPasswordAsync(Application.DTOs.Identity.ResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"No hay cuentas registradas con el correo {request.Email}"
                    );
                }

                // Decodificar el token
                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(request.Token)
                );

                var result = await _userManager.ResetPasswordAsync(
                    user,
                    decodedToken,
                    request.NewPassword
                );

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al restablecer la contraseña",
                        errors
                    );
                }

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Contraseña restablecida exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al restablecer la contraseña",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword
                );

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al cambiar la contraseña",
                        errors
                    );
                }

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Contraseña cambiada exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al cambiar la contraseña",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> SignOutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return ApiResponse<bool>.SuccessResponse(true, "Sesión cerrada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al cerrar sesión",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> UserExistsAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return ApiResponse<bool>.SuccessResponse(user != null);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al verificar usuario",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}

