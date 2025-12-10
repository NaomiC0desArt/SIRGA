using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class ComponenteCalificacionRepository : GenericRepository<ComponenteCalificacion>, IComponenteCalificacionRepository
    {
        public ComponenteCalificacionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ComponenteCalificacion>> GetByTipoAsignaturaAsync(string tipoAsignatura)
        {
            return await _context.ComponentesCalificacion
                .Where(c => c.TipoAsignatura == tipoAsignatura && c.Activo)
                .OrderBy(c => c.Orden)
                .ToListAsync();
        }

        public async Task<ComponenteCalificacion> GetByNombreYTipoAsync(string nombre, string tipoAsignatura)
        {
            return await _context.ComponentesCalificacion
                .FirstOrDefaultAsync(c => c.Nombre == nombre && c.TipoAsignatura == tipoAsignatura);
        }
    }
}
