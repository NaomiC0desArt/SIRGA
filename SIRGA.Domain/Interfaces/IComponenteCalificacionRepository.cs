using SIRGA.Domain.Entities;

namespace SIRGA.Domain.Interfaces
{
    public interface IComponenteCalificacionRepository : IGenericRepository<ComponenteCalificacion>
    {
        Task<List<ComponenteCalificacion>> GetByTipoAsignaturaAsync(string tipoAsignatura);
        Task<ComponenteCalificacion> GetByNombreYTipoAsync(string nombre, string tipoAsignatura);
    }
}
