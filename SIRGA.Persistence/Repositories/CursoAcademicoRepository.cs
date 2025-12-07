using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class CursoAcademicoRepository : GenericRepository<CursoAcademico>, ICursoAcademicoRepository
    {
        private readonly ApplicationDbContext _context;

        public CursoAcademicoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<CursoAcademico>> GetAllWithGradoAsync()
        {
            return await _dbSet
                .Include(c => c.Grado)
                .ToListAsync();
        }

        public async Task<CursoAcademico?> GetByIdWithGradoAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Grado)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
