using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Persistence.Repositories
{
    public class ClaseProgramadaRepository : IClaseProgramadaRepository
    {
        private readonly ApplicationDbContext _context;

        public ClaseProgramadaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClaseProgramada> AddAsync(ClaseProgramada claseProgramada)
        {
            await _context.ClasesProgramadas.AddAsync(claseProgramada);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(claseProgramada.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var claseProgramada = await GetByIdAsync(id);
            if (claseProgramada == null)
            {
                return false;
            }

            _context.ClasesProgramadas.Remove(claseProgramada);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ClaseProgramada>> GetAllAsync()
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.Profesor) 
                    .ThenInclude(p => p.ApplicationUser) 
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .ToListAsync();
        }

        public async Task<ClaseProgramada> GetByIdAsync(int id)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.Profesor) 
                    .ThenInclude(p => p.ApplicationUser)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ClaseProgramada> UpdateAsync(ClaseProgramada claseProgramada)
        {
            _context.ClasesProgramadas.Update(claseProgramada);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(claseProgramada.Id);
        }
    }
}
