using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class CalificacionDetalleRepository : GenericRepository<CalificacionDetalle>, ICalificacionDetalleRepository
    {
        public CalificacionDetalleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<CalificacionDetalle>> GetDetallesPorCalificacionAsync(int idCalificacion)
        {
            return await _context.CalificacionDetalles
                .Include(d => d.Componente)
                .Where(d => d.IdCalificacion == idCalificacion)
                .OrderBy(d => d.Componente.Orden)
                .ToListAsync();
        }

        public async Task DeleteDetallesPorCalificacionAsync(int idCalificacion)
        {
            var detalles = await _context.CalificacionDetalles
                .Where(d => d.IdCalificacion == idCalificacion)
                .ToListAsync();

            _context.CalificacionDetalles.RemoveRange(detalles);
            await _context.SaveChangesAsync();
        }
    }
}
