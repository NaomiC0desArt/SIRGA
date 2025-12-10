using SIRGA.Domain.Entities;

namespace SIRGA.Domain.Interfaces
{
    public interface IHistorialCalificacionRepository : IGenericRepository<HistorialCalificacion>
    {
        Task<List<HistorialCalificacion>> GetHistorialPorCalificacionAsync(int idCalificacion);
        Task RegistrarCambioAsync(HistorialCalificacion historial);
    }
}
