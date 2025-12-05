using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Estudiante;
using SIRGA.Application.DTOs.UserManagement;
using SIRGA.Application.Interfaces.Services;
using SIRGA.Application.Interfaces.Usuarios;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using SIRGA.Application.DTOs.Infrastructure;
using Microsoft.Extensions.Logging;
using SIRGA.Application.Interfaces.Services.Email;
using SIRGA.Identity.Shared.Interfaces;
using SIRGA.Identity.Shared.Enum;

namespace SIRGA.Application.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMatriculaGeneratorService _matriculaGenerator;
        private readonly IEmailGeneratorService _emailGenerator;
        private readonly IPasswordGeneratorService _passwordGenerator;
        private readonly IEmailService _emailService;          
        private readonly IEmailTemplateGenerator _templateGenerator;
        private readonly IIdentityUrlGenerator _urlGenerator;
        private readonly ILogger<IEmailService> _logger;

        public EstudianteService(
            IEstudianteRepository estudianteRepository,
            UserManager<ApplicationUser> userManager,
            IMatriculaGeneratorService matriculaGenerator,
        IEmailGeneratorService emailGenerator,
            IPasswordGeneratorService passwordGenerator,
            IEmailService emailService,
         IEmailTemplateGenerator templateGenerator,
         IIdentityUrlGenerator urlGenerator,
        ILogger<IEmailService> logger)
        {
            _estudianteRepository = estudianteRepository;
            _userManager = userManager;
            _matriculaGenerator = matriculaGenerator;
            _emailGenerator = emailGenerator;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _templateGenerator = templateGenerator;
            _urlGenerator = urlGenerator;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> ActivateAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(id);
                if (estudiante == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Estudiante no encontrado");
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario asociado no encontrado");
                }

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al activar el estudiante",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(true, "Estudiante activado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al activar el estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // ==================== NUEVO: DESACTIVAR ESTUDIANTE ====================
        public async Task<ApiResponse<bool>> DeactivateAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(id);
                if (estudiante == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Estudiante no encontrado");
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario asociado no encontrado");
                }

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al desactivar el estudiante",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(true, "Estudiante desactivado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al desactivar el estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<EstudianteResponseDto>> CreateEstudianteAsync(CreateEstudianteDto dto)
        {
            try
            {
                
                var matricula = await _matriculaGenerator.GenerateNextMatriculaAsync();

                
                var yearOfEntry = dto.YearOfEntry ?? DateTime.Now.Year;

                
                var email = _emailGenerator.GenerateEstudianteEmail(matricula);

                
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                        "Ya existe un usuario con ese correo electrónico."
                    );
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
                    DateOfEntry = DateOnly.FromDateTime(new DateTime(yearOfEntry, 1, 1)),
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    MustCompleteProfile = true, 
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false
                };

                var result = await _userManager.CreateAsync(applicationUser, temporaryPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                        "Error al crear el usuario",
                        errors
                    );
                }

                
                await _userManager.AddToRoleAsync(applicationUser, RolesEnum.Estudiante.ToString());

                
                var estudiante = new Estudiante
                {
                    Matricula = matricula,
                    ApplicationUserId = applicationUser.Id,
                    MedicalConditions = string.Empty,
                    MedicalNote = string.Empty,
                    EmergencyContactName = string.Empty,
                    EmergencyContactPhone = string.Empty
                };

                await _estudianteRepository.AddAsync(estudiante);

                try
                {
                    var emailBody = _templateGenerator.GenerateWelcomeEmail(
                        dto.FirstName,
                        email,
                        temporaryPassword,
                        "Estudiante"
                    );

                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        To = email,
                        Subject = "Bienvenido a SIRGA - Tus credenciales de acceso",
                        Body = emailBody
                    });

                    _logger?.LogInformation($"Email de bienvenida enviado a {email}");
                }
                catch (Exception emailEx)
                {
                    
                    _logger?.LogError(emailEx, $"Error enviando email a {email}");
                }

                
                var response = new EstudianteResponseDto
                {
                    Id = estudiante.Id,
                    Matricula = estudiante.Matricula,
                    Email = applicationUser.Email,
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    Gender = applicationUser.Gender,
                    DateOfBirth = applicationUser.DateOfBirth,
                    Province = applicationUser.Province,
                    Sector = applicationUser.Sector,
                    Address = applicationUser.Address,
                    PhoneNumber = applicationUser.PhoneNumber,
                    DateOfEntry = applicationUser.DateOfEntry,
                    IsActive = applicationUser.IsActive,
                    MustCompleteProfile = applicationUser.MustCompleteProfile,
                    ApplicationUserId = applicationUser.Id,
                    TemporaryPassword = temporaryPassword,
                };

                return ApiResponse<EstudianteResponseDto>.SuccessResponse(
                    response,
                    $"Estudiante creado exitosamente. Matrícula: {matricula}, Email: {email}"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                    "Error al crear el estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> CompleteProfileAsync(string userId, CompleteStudentProfileDto dto)
        {
            try
            {
                
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                
                var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);
                if (estudiante == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Estudiante no encontrado");
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

                
                estudiante.MedicalConditions = dto.MedicalConditions ?? string.Empty;
                estudiante.MedicalNote = dto.MedicalNote ?? string.Empty;
                estudiante.EmergencyContactName = dto.EmergencyContactName;
                estudiante.EmergencyContactPhone = dto.EmergencyContactPhone;

                await _estudianteRepository.UpdateAsync(estudiante);

                
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
        public async Task<ApiResponse<EstudianteResponseDto>> GetEstudianteByIdAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(id);
                if (estudiante == null)
                {
                    return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                        "Estudiante no encontrado"
                    );
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                        "Usuario asociado no encontrado"
                    );
                }

                var response = MapToResponseDto(estudiante, user);
                return ApiResponse<EstudianteResponseDto>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                    "Error al obtener el estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<EstudianteResponseDto>>> GetAllEstudiantesAsync()
        {
            try
            {
                var estudiantes = await _estudianteRepository.GetAllAsync();
                _logger.LogInformation($"Total estudiantes en BD: {estudiantes.Count}");

                var responseDtos = new List<EstudianteResponseDto>();

                foreach (var estudiante in estudiantes)
                {
                    _logger.LogInformation($"Buscando usuario para estudiante ID {estudiante.Id}, ApplicationUserId={estudiante.ApplicationUserId}");
                    var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);

                    if (user != null)
                    {
                        responseDtos.Add(MapToResponseDto(estudiante, user));
                    }
                    else
                    {
                        _logger.LogWarning($"Usuario no encontrado para ApplicationUserId={estudiante.ApplicationUserId}");
                    }
                }

                _logger.LogInformation($"Estudiantes válidos devueltos: {responseDtos.Count}");
                return ApiResponse<List<EstudianteResponseDto>>.SuccessResponse(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAllEstudiantesAsync");
                throw; 
            }
        }

        public async Task<ApiResponse<EstudianteResponseDto>> UpdateEstudianteAsync(int id, UpdateEstudianteDto dto)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(id);
                if (estudiante == null)
                {
                    return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                        "Estudiante no encontrado"
                    );
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                        "Usuario asociado no encontrado"
                    );
                }

                // tipos de referencia
                user.FirstName = dto.FirstName ?? user.FirstName;
                user.LastName = dto.LastName ?? user.LastName;
                user.Province = dto.Province ?? user.Province;
                user.Sector = dto.Sector ?? user.Sector;
                user.Address = dto.Address ?? user.Address;
                user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

                // valor anulables 
                user.Gender = dto.Gender.HasValue ? (char)dto.Gender : user.Gender;
                user.DateOfBirth = dto.DateOfBirth.HasValue ? (DateOnly)dto.DateOfBirth : user.DateOfBirth;
                user.IsActive = dto.IsActive.HasValue ? (bool)dto.IsActive : user.IsActive;

                await _userManager.UpdateAsync(user);

  

                await _estudianteRepository.UpdateAsync(estudiante);

                var response = MapToResponseDto(estudiante, user);
                return ApiResponse<EstudianteResponseDto>.SuccessResponse(
                    response,
                    "Estudiante actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<EstudianteResponseDto>.ErrorResponse(
                    "Error al actualizar el estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteEstudianteAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(id);
                if (estudiante == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Estudiante no encontrado");
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                await _estudianteRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Estudiante eliminado exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar el estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CredentialsDto>> GetCredentialsByEstudianteIdAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(id);
                if (estudiante == null)
                {
                    return ApiResponse<CredentialsDto>.ErrorResponse("Estudiante no encontrado");
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                if (user == null)
                {
                    return ApiResponse<CredentialsDto>.ErrorResponse("Usuario asociado no encontrado");
                }

                var credentials = new CredentialsDto
                {
                    Email = user.Email,
                    Matricula = estudiante.Matricula,
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

        private EstudianteResponseDto MapToResponseDto(Estudiante estudiante, ApplicationUser user)
        {
            return new EstudianteResponseDto
            {
                Id = estudiante.Id,
                Matricula = estudiante.Matricula,
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
                EmergencyContactName = estudiante.EmergencyContactName,
                EmergencyContactPhone = estudiante.EmergencyContactPhone,
                MustCompleteProfile = user.MustCompleteProfile,
                ApplicationUserId = user.Id
            };
        }

    }
}
