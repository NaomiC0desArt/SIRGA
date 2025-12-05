

using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using System.Linq.Expressions;

namespace SIRGA.Persistence.Repositories
{
    public class InscripcionActividadRepository : IInscripcionActividadRepository
    {
        private readonly ApplicationDbContext _context;

        public InscripcionActividadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InscripcionActividad> AddAsync(InscripcionActividad entity)
        {
            await _context.InscripcionesActividades.AddAsync(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inscripcion = await GetByIdAsync(id);
            if (inscripcion == null) return false;

            _context.InscripcionesActividades.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Expression<Func<InscripcionActividad, bool>> predicate)
        {
            return await _context.InscripcionesActividades.AnyAsync(predicate);
        }

        public async Task<List<InscripcionActividad>> GetAllAsync()
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Estudiante)
                .Include(i => i.Actividad)
                .ToListAsync();
        }

        public async Task<List<InscripcionActividad>> GetAllByConditionAsync(Expression<Func<InscripcionActividad, bool>> predicate)
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Estudiante)
                .Include(i => i.Actividad)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<InscripcionActividad> GetByConditionAsync(Expression<Func<InscripcionActividad, bool>> predicate)
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Estudiante)
                .Include(i => i.Actividad)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<InscripcionActividad> GetByIdAsync(int id)
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Estudiante)
                .Include(i => i.Actividad)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<InscripcionActividad> UpdateAsync(InscripcionActividad entity)
        {
            _context.InscripcionesActividades.Update(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<InscripcionActividad> GetInscripcionActivaAsync(int idEstudiante, int idActividad)
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Estudiante)
                .Include(i => i.Actividad)
                .FirstOrDefaultAsync(i =>
                    i.IdEstudiante == idEstudiante &&
                    i.IdActividad == idActividad &&
                    i.EstaActiva);
        }

        public async Task<List<InscripcionActividad>> GetInscripcionesPorEstudianteAsync(int idEstudiante)
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Actividad)
                    .ThenInclude(a => a.ProfesorEncargado)
                .Where(i => i.IdEstudiante == idEstudiante && i.EstaActiva)
                .ToListAsync();
        }

        public async Task<List<InscripcionActividad>> GetInscripcionesPorActividadAsync(int idActividad)
        {
            return await _context.InscripcionesActividades
                .Include(i => i.Estudiante)
                .Where(i => i.IdActividad == idActividad && i.EstaActiva)
                .ToListAsync();
        }

        public async Task<bool> EstaInscritoAsync(int idEstudiante, int idActividad)
        {
            return await _context.InscripcionesActividades
                .AnyAsync(i =>
                    i.IdEstudiante == idEstudiante &&
                    i.IdActividad == idActividad &&
                    i.EstaActiva);
        }
    }
}
