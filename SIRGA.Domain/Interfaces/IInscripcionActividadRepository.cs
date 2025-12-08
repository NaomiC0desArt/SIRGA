using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
using SIRGA.Domain.ReadModels;
namespace SIRGA.Domain.Interfaces
{
    public interface IInscripcionActividadRepository : IGenericRepository<InscripcionActividad>
    {
        Task<bool> EstaInscritoAsync(int idEstudiante, int idActividad);
        Task<InscripcionActividad> GetInscripcionActivaAsync(int idEstudiante, int idActividad);

        
        Task<List<InscripcionActividadConDetalles>> GetInscripcionesPorActividadConDetallesAsync(int idActividad);
        Task<List<InscripcionActividadConDetalles>> GetInscripcionesPorEstudianteConDetallesAsync(int idEstudiante);

        
        Task<List<InscripcionActividad>> GetInscripcionesPorActividadAsync(int idActividad);
    }
}
