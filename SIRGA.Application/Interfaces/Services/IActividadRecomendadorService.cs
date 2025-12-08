using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.IA;

namespace SIRGA.Application.Interfaces.Services
{
    public interface IActividadRecomendadorService
    {
        /// Recomienda actividades extracurriculares basadas en el perfil del estudiante
        Task<ApiResponse<List<ActividadRecomendadaDto>>> RecomendarActividadesAsync(int idEstudiante);
        /// Obtiene estadísticas de popularidad de actividades
        Task<ApiResponse<List<EstadisticaActividadDto>>> ObtenerEstadisticasActividadesAsync(int idCursoAcademico);
    }
}
