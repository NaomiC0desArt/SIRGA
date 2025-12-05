
using Microsoft.AspNetCore.Http;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.ActividadExtracurricular;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IActividadExtracurricularService
    {
        // CRUD Admin
        Task<ApiResponse<ActividadExtracurricularDto>> CreateAsync(CreateActividadDto dto, IFormFile imagen = null);
        Task<ApiResponse<ActividadExtracurricularDto>> UpdateAsync(int id, UpdateActividadDto dto, IFormFile imagen = null);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<ActividadDetalleDto>> GetByIdAsync(int id, int? estudianteId = null);
        Task<ApiResponse<List<ActividadExtracurricularDto>>> GetAllAsync(int? estudianteId = null);
        Task<ApiResponse<List<ActividadExtracurricularDto>>> GetActividadesActivasAsync(int? estudianteId = null);
        Task<ApiResponse<List<ActividadExtracurricularDto>>> GetPorCategoriaAsync(string categoria, int? estudianteId = null);

        // Gestión de Inscripciones (Admin)
        Task<ApiResponse<List<EstudianteInscritoDto>>> GetEstudiantesInscritosAsync(int idActividad);
        Task<ApiResponse<bool>> InscribirEstudianteAsync(int idActividad, int idEstudiante);
        Task<ApiResponse<bool>> DesinscribirEstudianteAsync(int idActividad, int idEstudiante);

        // Acciones del Estudiante
        Task<ApiResponse<bool>> InscribirseAsync(int idActividad, int idEstudiante);
        Task<ApiResponse<bool>> DesinscribirseAsync(int idActividad, int idEstudiante);
        Task<ApiResponse<bool>> EstaInscritoAsync(int idActividad, int idEstudiante);
    }
}
