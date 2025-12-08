using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Estudiante;
using SIRGA.Application.DTOs.UserManagement;


namespace SIRGA.Application.Interfaces.Usuarios
{
    public interface IEstudianteService
    {
        Task<ApiResponse<EstudianteResponseDto>> CreateEstudianteAsync(CreateEstudianteDto dto);
        Task<ApiResponse<EstudianteResponseDto>> GetEstudianteByIdAsync(int id);
        Task<ApiResponse<List<EstudianteResponseDto>>> GetAllEstudiantesAsync();
        Task<ApiResponse<EstudianteResponseDto>> UpdateEstudianteAsync(int id, UpdateEstudianteDto dto);
        Task<ApiResponse<bool>> DeleteEstudianteAsync(int id);
        Task<ApiResponse<int>> GetEstudianteIdByUserIdAsync(string applicationUserId);
        Task<ApiResponse<bool>> ActivateAsync(int id);
        Task<ApiResponse<bool>> DeactivateAsync(int id);
        Task<ApiResponse<CredentialsDto>> GetCredentialsByEstudianteIdAsync(int id);
        Task<ApiResponse<bool>> CompleteProfileAsync(string userId, CompleteStudentProfileDto dto);
    }
}
