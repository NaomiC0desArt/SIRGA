using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Persistence.Repositories
{
    public class CursoAcademicoRepository : ICursoAcademicoRepository
    {
        private readonly ApplicationDbContext _context;

        public CursoAcademicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CursoAcademico> AddAsync(CursoAcademico cursoAcademico)
        {
            await _context.CursosAcademicos.AddAsync(cursoAcademico);
            await _context.SaveChangesAsync();
            return cursoAcademico;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cursoAcademinico = await GetByIdAsync(id);
            if (cursoAcademinico == null) { return false; }

            _context.CursosAcademicos.Remove(cursoAcademinico);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CursoAcademico>> GetAllAsync()
        {
            return await _context.CursosAcademicos.ToListAsync();
        }

        public async Task<CursoAcademico> GetByIdAsync(int id)
        {
            return await _context.CursosAcademicos.FindAsync(id);
        }

        public async Task<CursoAcademico> UpdateAsync(CursoAcademico cursoAcademico)
        {
            _context.CursosAcademicos.Update(cursoAcademico);
            await _context.SaveChangesAsync();
            return cursoAcademico;
        }
    }
}
