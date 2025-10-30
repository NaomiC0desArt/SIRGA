using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Profesor;
using SIRGA.Application.DTOs.UserManagement;
using SIRGA.Application.DTOs.UserManagement.Estudiante;


namespace SIRGA.Application.Interfaces.Usuarios
{
    public interface IProfesorService
    {
        Task<ApiResponse<ProfesorResponseDto>> CreateProfesorAsync(CreateProfesorDto dto);
        Task<ApiResponse<ProfesorResponseDto>> GetProfesorByIdAsync(int id);
        Task<ApiResponse<List<ProfesorResponseDto>>> GetAllProfesoresAsync();
        Task<ApiResponse<ProfesorResponseDto>> UpdateProfesorAsync(int id, UpdateProfesorDto dto);
        Task<ApiResponse<bool>> DeleteProfesorAsync(int id);
        Task<ApiResponse<CredentialsDto>> GetCredentialsByProfesorIdAsync(int id);
        Task<ApiResponse<bool>> CompleteProfileAsync(string userId, CompleteTeacherProfileDto dto);
    }
}
