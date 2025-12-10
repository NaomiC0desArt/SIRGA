using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Calificacion;
using SIRGA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface ICalificacionService
    {
        Task<ApiResponse<List<AsignaturaProfesorDto>>> GetAsignaturasProfesorAsync(string applicationUserId);
        Task<ApiResponse<CapturaMasivaDto>> GetEstudiantesParaCalificarAsync(string applicationUserId, int idAsignatura, int idCursoAcademico);
        Task<ApiResponse<bool>> GuardarCalificacionesAsync(CapturaMasivaDto dto);
        Task<ApiResponse<bool>> PublicarCalificacionesAsync(PublicarCalificacionesDto dto);
        Task<ApiResponse<List<CalificacionEstudianteViewDto>>> GetCalificacionesEstudianteAsync(string applicationUserId);
        Task<ApiResponse<bool>> EditarCalificacionAsync(EditarCalificacionDto dto, string usuarioId, string usuarioNombre, string rol);
        Task<ApiResponse<List<HistorialCalificacion>>> GetHistorialCalificacionAsync(int idCalificacion);
    }
}
