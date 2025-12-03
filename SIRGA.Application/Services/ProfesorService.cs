using Microsoft.AspNetCore.Identity;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Profesor;
using SIRGA.Application.DTOs.UserManagement;
using SIRGA.Application.Interfaces.Services;
using SIRGA.Application.Interfaces.Usuarios;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Application.DTOs.Infrastructure;
using SIRGA.Application.Interfaces.Services.Email;
using SIRGA.Identity.Shared.Interfaces;
using SIRGA.Identity.Shared.Enum;


namespace SIRGA.Application.Services
{
    public class ProfesorService : IProfesorService
    {
        private readonly IProfesorRepository _profesorRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailGeneratorService _emailGenerator;
        private readonly IPasswordGeneratorService _passwordGenerator;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateGenerator _templateGenerator;
        private readonly IIdentityUrlGenerator _urlGenerator;

        public ProfesorService(
            IProfesorRepository profesorRepository,
            UserManager<ApplicationUser> userManager,
            IEmailGeneratorService emailGenerator,
            IPasswordGeneratorService passwordGenerator,
            IEmailService emailService,                        
        IEmailTemplateGenerator templateGenerator,
         IIdentityUrlGenerator urlGenerator)
        {
            _profesorRepository = profesorRepository;
            _userManager = userManager;
            _emailGenerator = emailGenerator;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;                      
            _templateGenerator = templateGenerator;
            _urlGenerator = urlGenerator;
        }

       
        public async Task<ApiResponse<bool>> ActivateAsync(int id)
        {
            try
            {
                var profesor = await _profesorRepository.GetByIdAsync(id);
                if (profesor == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Profesor no encontrado");
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario asociado no encontrado");
                }

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al activar el profesor",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(true, "Profesor activado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al activar el profesor",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeactivateAsync(int id)
        {
            try
            {
                var profesor = await _profesorRepository.GetByIdAsync(id);
                if (profesor == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Profesor no encontrado");
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario asociado no encontrado");
                }

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al desactivar el profesor",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(true, "Profesor desactivado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al desactivar el profesor",
                    new List<string> { ex.Message }
                );
            }
        }
        public async Task<ApiResponse<ProfesorResponseDto>> CreateProfesorAsync(CreateProfesorDto dto)
        {
            try
            {
                
                var email = _emailGenerator.GenerateProfesorEmail(dto.FirstName, dto.LastName);

                
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // Si el email ya existe, agregar un número al final
                    int counter = 1;
                    string baseEmail = email.Replace("@SIGA.edu.do", "");
                    while (existingUser != null)
                    {
                        email = $"{baseEmail}{counter}@SIGA.edu.do";
                        existingUser = await _userManager.FindByEmailAsync(email);
                        counter++;
                    }
                }

                
                var temporaryPassword = _passwordGenerator.GenerateTemporaryPassword();

                
                var applicationUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = 'O', 
                    DateOfBirth = DateOnly.MinValue,
                    Province = "Por completar",
                    Sector = "Por completar",
                    Address = "Por completar",
                    PhoneNumber = null,

                    IsActive = true,
                    DateOfEntry = DateOnly.MinValue,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    MustCompleteProfile = true,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false
                };

                var result = await _userManager.CreateAsync(applicationUser, temporaryPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Error al crear el usuario",
                        errors
                    );
                }

                
                await _userManager.AddToRoleAsync(applicationUser, RolesEnum.Profesor.ToString());

                
                var profesor = new Profesor
                {
                    ApplicationUserId = applicationUser.Id,
                    Specialty = dto.Specialty ?? string.Empty
                };

                await _profesorRepository.AddAsync(profesor);

                try
                {
                    var emailBody = _templateGenerator.GenerateWelcomeEmail(
                        dto.FirstName,
                        email,
                        temporaryPassword,
                        "Profesor"
                    );

                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        To = email,
                        Subject = "Bienvenido a SIRGA - Tus credenciales de acceso",
                        Body = emailBody
                    });
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine("Error al enviar el email de bienvenida: " + emailEx.Message);
                }

                // retornar respuesta
                var response = new ProfesorResponseDto
                {
                    Id = profesor.Id,
                    Email = applicationUser.Email,
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    Gender = applicationUser.Gender,
                    DateOfBirth = applicationUser.DateOfBirth,
                    Province = applicationUser.Province,
                    Sector = applicationUser.Sector,
                    Address = applicationUser.Address,
                    DateOfEntry = applicationUser.DateOfEntry,
                    IsActive = applicationUser.IsActive,
                    MustCompleteProfile = applicationUser.MustCompleteProfile,
                    Specialty = profesor.Specialty,
                    ApplicationUserId = applicationUser.Id,
                    TemporaryPassword = temporaryPassword
                };

                return ApiResponse<ProfesorResponseDto>.SuccessResponse(
                    response,
                    $"Profesor creado exitosamente. Email: {email}"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                    "Error al crear el profesor",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> CompleteProfileAsync(string userId, CompleteTeacherProfileDto dto)
        {
            try
            {
                
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                
                var profesor = await _profesorRepository.GetByApplicationUserIdAsync(userId);
                if (profesor == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Profesor no encontrado");
                }

                
                user.Gender = dto.Gender;
                user.DateOfBirth = dto.DateOfBirth;
                user.Province = dto.Province;
                user.Sector = dto.Sector;
                user.Address = dto.Address;
                user.PhoneNumber = dto.PhoneNumber;
                user.MustCompleteProfile = false; 

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al actualizar el perfil",
                        updateResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                
                profesor.Specialty = dto.Specialty ?? string.Empty;

                await _profesorRepository.UpdateAsync(profesor);

                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return ApiResponse<bool>.ErrorResponse("Las contraseñas no coinciden");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordChangeResult = await _userManager.ResetPasswordAsync(
                    user,
                    token, 
                    dto.NewPassword
                );

                if (!passwordChangeResult.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al cambiar la contraseña",
                        passwordChangeResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Perfil completado exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al completar el perfil",
                    new List<string> { ex.Message }
                );
            }
        }
        public async Task<ApiResponse<ProfesorResponseDto>> GetProfesorByIdAsync(int id)
        {
            try
            {
                var profesor = await _profesorRepository.GetByIdAsync(id);
                if (profesor == null)
                {
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Profesor no encontrado"
                    );
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Usuario asociado no encontrado"
                    );
                }

                var response = MapToResponseDto(profesor, user);
                return ApiResponse<ProfesorResponseDto>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                    "Error al obtener el profesor",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<ProfesorResponseDto>>> GetAllProfesoresAsync()
        {
            try
            {
                var profesores = await _profesorRepository.GetAllAsync();
                var responseDtos = new List<ProfesorResponseDto>();

                foreach (var profesor in profesores)
                {
                    var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                    if (user != null)
                    {
                        responseDtos.Add(MapToResponseDto(profesor, user));
                    }
                }

                return ApiResponse<List<ProfesorResponseDto>>.SuccessResponse(responseDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProfesorResponseDto>>.ErrorResponse(
                    "Error al obtener los profesores",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<ProfesorResponseDto>> UpdateProfesorAsync(int id, UpdateProfesorDto dto)
        {
            try
            {
                var profesor = await _profesorRepository.GetByIdAsync(id);
                if (profesor == null)
                {
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Profesor no encontrado"
                    );
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Usuario asociado no encontrado"
                    );
                }

                
                user.FirstName = dto.FirstName ?? user.FirstName;
                user.LastName = dto.LastName ?? user.LastName;
                user.Province = dto.Province ?? user.Province;
                user.Sector = dto.Sector ?? user.Sector;
                user.Address = dto.Address ?? user.Address;
                user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;


                user.Gender = dto.Gender.HasValue ? (char)dto.Gender : user.Gender;
                user.DateOfBirth = dto.DateOfBirth.HasValue ? (DateOnly)dto.DateOfBirth : user.DateOfBirth;
                user.IsActive = dto.IsActive.HasValue ? (bool)dto.IsActive : user.IsActive;

                await _userManager.UpdateAsync(user);

                // Actualizar Profesor
                profesor.Specialty = dto.Specialty ?? profesor.Specialty;

                await _profesorRepository.UpdateAsync(profesor);

                var response = MapToResponseDto(profesor, user);
                return ApiResponse<ProfesorResponseDto>.SuccessResponse(
                    response,
                    "Profesor actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                    "Error al actualizar el profesor",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteProfesorAsync(int id)
        {
            try
            {
                var profesor = await _profesorRepository.GetByIdAsync(id);
                if (profesor == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Profesor no encontrado");
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                await _profesorRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Profesor eliminado exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar el profesor",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CredentialsDto>> GetCredentialsByProfesorIdAsync(int id)
        {
            try
            {
                var profesor = await _profesorRepository.GetByIdAsync(id);
                if (profesor == null)
                {
                    return ApiResponse<CredentialsDto>.ErrorResponse("Profesor no encontrado");
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<CredentialsDto>.ErrorResponse("Usuario asociado no encontrado");
                }

                var credentials = new CredentialsDto
                {
                    Email = user.Email,
                    TemporaryPassword = "********" 
                };

                return ApiResponse<CredentialsDto>.SuccessResponse(credentials);
            }
            catch (Exception ex)
            {
                return ApiResponse<CredentialsDto>.ErrorResponse(
                    "Error al obtener las credenciales",
                    new List<string> { ex.Message }
                );
            }
        }
        public async Task<ApiResponse<ProfesorResponseDto>> GetProfesorByApplicationUserIdAsync(string applicationUserId)
        {
            try
            {
                var profesor = await _profesorRepository.GetByApplicationUserIdAsync(applicationUserId);
                if (profesor == null)
                {
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Profesor no encontrado"
                    );
                }

                var user = await _userManager.FindByIdAsync(profesor.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                        "Usuario asociado no encontrado"
                    );
                }

                var response = MapToResponseDto(profesor, user);
                return ApiResponse<ProfesorResponseDto>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfesorResponseDto>.ErrorResponse(
                    "Error al obtener el profesor",
                    new List<string> { ex.Message }
                );
            }
        }
        private ProfesorResponseDto MapToResponseDto(Profesor profesor, ApplicationUser user)
        {
            return new ProfesorResponseDto
            {
                Id = profesor.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Province = user.Province,
                Sector = user.Sector,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                DateOfEntry = user.DateOfEntry,
                IsActive = user.IsActive,
                MustCompleteProfile = user.MustCompleteProfile,
                Specialty = profesor.Specialty,
                ApplicationUserId = user.Id
            };
        }
    }
}
