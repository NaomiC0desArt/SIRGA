using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Persistence.Repositories
{
    public class AsignaturaRepository : IAsignaturaRepository
    {
        private readonly ApplicationDbContext _context;

        public AsignaturaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Asignatura> AddAsync(Asignatura asignatura)
        {
            await _context.Asignaturas.AddAsync(asignatura);
            await _context.SaveChangesAsync();
            return asignatura;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var asignatura = await GetByIdAsync(id);
            if (asignatura == null)
            {
                return false;
            }

            _context.Asignaturas.Remove(asignatura);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Asignatura>> GetAllAsync()
        {
            return await _context.Asignaturas.ToListAsync();
        }

        public async Task<Asignatura> GetByIdAsync(int id)
        {
            return await _context.Asignaturas.FindAsync(id);
        }

        public async Task<Asignatura> UpdateAsync(Asignatura asignatura)
        {
            _context.Asignaturas.Update(asignatura);
            await _context.SaveChangesAsync();
            return asignatura;
        }
    }
}
