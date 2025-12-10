using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class CursoAcademicoRepository : GenericRepository<CursoAcademico>, ICursoAcademicoRepository
    {
        public CursoAcademicoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<CursoAcademico>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.Grado)
                .Include(c => c.Seccion)
                .Include(c => c.AnioEscolar)
                .Include(c => c.AulaBase)
                .OrderBy(c => c.IdAnioEscolar)
                .ThenBy(c => c.IdGrado)
                .ThenBy(c => c.IdSeccion)
                .ToListAsync();
        }

        public async Task<CursoAcademico> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Grado)
                .Include(c => c.Seccion)
                .Include(c => c.AnioEscolar)
                .Include(c => c.AulaBase)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CursoAcademico>> GetByAnioEscolarAsync(int idAnioEscolar)
        {
            return await _dbSet
                .Include(c => c.Grado)
                .Include(c => c.Seccion)
                .Include(c => c.AnioEscolar)
                .Include(c => c.AulaBase)
                .Where(c => c.IdAnioEscolar == idAnioEscolar)
                .OrderBy(c => c.IdGrado)
                .ThenBy(c => c.IdSeccion)
                .ToListAsync();
        }

        public async Task<List<CursoAcademico>> GetByGradoAsync(int idGrado)
        {
            return await _dbSet
                .Include(c => c.Grado)
                .Include(c => c.Seccion)
                .Include(c => c.AnioEscolar)
                .Include(c => c.AulaBase)
                .Where(c => c.IdGrado == idGrado)
                .OrderBy(c => c.IdAnioEscolar)
                .ThenBy(c => c.IdSeccion)
                .ToListAsync();
        }

        public async Task<bool> ExisteCursoAsync(int idGrado, int idSeccion, int idAnioEscolar, int? excludeId = null)
        {
            var query = _dbSet.Where(c =>
                c.IdGrado == idGrado &&
                c.IdSeccion == idSeccion &&
                c.IdAnioEscolar == idAnioEscolar);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        public async Task<int> GetCantidadEstudiantesEnSeccionAsync(int idSeccion, int idAnioEscolar)
        {
            // Implementar cuando tengas la entidad Estudiante
            // Por ahora retorna 0
            return await Task.FromResult(0);
        }
    }
}
