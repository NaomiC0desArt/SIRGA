using SIRGA.Domain.Entities;

namespace SIRGA.Domain.Interfaces
{
    public interface ICalificacionDetalleRepository : IGenericRepository<CalificacionDetalle>
    {
        Task<List<CalificacionDetalle>> GetDetallesPorCalificacionAsync(int idCalificacion);
        Task DeleteDetallesPorCalificacionAsync(int idCalificacion);
    }
}
