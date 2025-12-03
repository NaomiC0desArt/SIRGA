
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Asistencia;
using SIRGA.Application.DTOs.ResponseDto;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IAsistenciaService
    {
        // Registrar asistencia
        Task<ApiResponse<AsistenciaResponseDto>> RegistrarAsistenciaAsync(RegistrarAsistenciaDto dto, string registradoPorId);
        Task<ApiResponse<List<AsistenciaResponseDto>>> RegistrarAsistenciaMasivaAsync(RegistrarAsistenciaMasivaDto dto, string registradoPorId);

        // Actualizar asistencia (solo admin)
        Task<ApiResponse<AsistenciaResponseDto>> ActualizarAsistenciaAsync(int id, ActualizarAsistenciaDto dto, string modificadoPorId);

        // Justificar asistencia (solo admin)
        Task<ApiResponse<AsistenciaResponseDto>> JustificarAsistenciaAsync(int id, JustificarAsistenciaDto dto, string usuarioJustificacionId);

        // Consultas para profesores
        Task<ApiResponse<List<ClaseDelDiaDto>>> GetClasesDelDiaAsync(int idProfesor, DateTime fecha);
        Task<ApiResponse<List<EstudianteClaseDto>>> GetEstudiantesPorClaseAsync(int idClaseProgramada, DateTime fecha);

        // Consultas de historial
        Task<ApiResponse<List<AsistenciaResponseDto>>> GetHistorialAsistenciaEstudianteAsync(int idEstudiante, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<ApiResponse<List<AsistenciaResponseDto>>> GetHistorialAsistenciaClaseAsync(int idClaseProgramada, DateTime fechaInicio, DateTime fechaFin);
        Task<ApiResponse<List<AsistenciaResponseDto>>> GetAsistenciasRequierenJustificacionAsync();

        // Estadísticas
        Task<ApiResponse<EstadisticasAsistenciaDto>> GetEstadisticasEstudianteAsync(int idEstudiante, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        // Consulta individual
        Task<ApiResponse<AsistenciaResponseDto>> GetAsistenciaByIdAsync(int id);

        // Eliminar (solo admin)
        Task<ApiResponse<bool>> EliminarAsistenciaAsync(int id);
    }
}
