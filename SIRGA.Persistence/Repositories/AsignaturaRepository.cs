using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class AsignaturaRepository : GenericRepository<Asignatura>, IAsignaturaRepository
    {
        public AsignaturaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<int> GetProfesoresCountAsync(int asignaturaId)
        {
            return await _context.ClasesProgramadas
                .Where(cp => cp.IdAsignatura == asignaturaId)
                .Select(cp => cp.IdProfesor)
                .Distinct()
                .CountAsync();
        }
    }
}
