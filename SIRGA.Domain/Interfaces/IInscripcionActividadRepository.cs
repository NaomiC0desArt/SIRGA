using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
namespace SIRGA.Domain.Interfaces
{
    public interface IInscripcionActividadRepository : IBaseRepository<InscripcionActividad>
    {
        Task<InscripcionActividad> GetInscripcionActivaAsync(int idEstudiante, int idActividad);
        Task<List<InscripcionActividad>> GetInscripcionesPorEstudianteAsync(int idEstudiante);
        Task<List<InscripcionActividad>> GetInscripcionesPorActividadAsync(int idActividad);
        Task<bool> EstaInscritoAsync(int idEstudiante, int idActividad);
    }
}
