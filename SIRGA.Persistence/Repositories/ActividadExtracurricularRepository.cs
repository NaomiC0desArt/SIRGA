using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Repositories
{
    public class ActividadExtracurricularRepository : IActividadExtracurricularRepository
    {
        private readonly ApplicationDbContext _context;

        public ActividadExtracurricularRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActividadExtracurricular> AddAsync(ActividadExtracurricular entity)
        {
            await _context.ActividadesExtracurriculares.AddAsync(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var actividad = await GetByIdAsync(id);
            if (actividad == null) return false;

            _context.ActividadesExtracurriculares.Remove(actividad);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Expression<Func<ActividadExtracurricular, bool>> predicate)
        {
            return await _context.ActividadesExtracurriculares.AnyAsync(predicate);
        }

        public async Task<List<ActividadExtracurricular>> GetAllAsync()
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                .ToListAsync();
        }

        public async Task<List<ActividadExtracurricular>> GetAllByConditionAsync(Expression<Func<ActividadExtracurricular, bool>> predicate)
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<ActividadExtracurricular> GetByConditionAsync(Expression<Func<ActividadExtracurricular, bool>> predicate)
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<ActividadExtracurricular> GetByIdAsync(int id)
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ActividadExtracurricular> UpdateAsync(ActividadExtracurricular entity)
        {
            _context.ActividadesExtracurriculares.Update(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<List<ActividadExtracurricular>> GetActividadesActivasAsync()
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                .Where(a => a.EstaActiva)
                .ToListAsync();
        }

        public async Task<List<ActividadExtracurricular>> GetActividadesPorCategoriaAsync(string categoria)
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                .Where(a => a.EstaActiva && a.Categoria == categoria)
                .ToListAsync();
        }

        public async Task<ActividadExtracurricular> GetActividadConDetallesAsync(int id)
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.ProfesorEncargado)
                .Include(a => a.Inscripciones)
                    .ThenInclude(i => i.Estudiante)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Estudiante>> GetEstudiantesInscritosAsync(int idActividad)
        {
            return await _context.InscripcionesActividades
                .Where(i => i.IdActividad == idActividad && i.EstaActiva)
                .Include(i => i.Estudiante)
                .Select(i => i.Estudiante)
                .ToListAsync();
        }
    }
}
