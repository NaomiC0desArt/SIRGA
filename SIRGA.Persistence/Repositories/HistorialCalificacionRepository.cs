using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class HistorialCalificacionRepository : GenericRepository<HistorialCalificacion>, IHistorialCalificacionRepository
    {
        public HistorialCalificacionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<HistorialCalificacion>> GetByCalificacionIdAsync(int idCalificacion)
        {
            return await _context.HistorialCalificaciones
                .Where(h => h.IdCalificacion == idCalificacion)
                .OrderByDescending(h => h.FechaModificacion)
                .ToListAsync();
        }
    }
}
