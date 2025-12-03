using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using System.Linq.Expressions;

namespace SIRGA.Persistence.Repositories
{
    public class InscripcionRepository : IInscripcionRepository
    {
        private readonly ApplicationDbContext _context;

        public InscripcionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Inscripcion> AddAsync(Inscripcion inscripcion)
        {
            await _context.Inscripciones.AddAsync(inscripcion);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(inscripcion.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inscripcion = await GetByIdAsync(id);
            if (inscripcion == null) { return false; }

            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Inscripcion>> GetAllAsync()
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .ToListAsync();
        }

        public async Task<Inscripcion> GetByIdAsync(int id)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Inscripcion> UpdateAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Update(inscripcion);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(inscripcion.Id);
        }

        public async Task<List<Inscripcion>> GetAllByConditionAsync(Expression<Func<Inscripcion, bool>> predicate)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Inscripcion> GetByConditionAsync(Expression<Func<Inscripcion, bool>> predicate)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Inscripcion, bool>> predicate)
        {
            return await _context.Inscripciones.AnyAsync(predicate);
        }
    }
}
