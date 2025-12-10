using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class AulaRepository : GenericRepository<Aula>, IAulaRepository
    {
        public AulaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Aula>> GetAulasDisponiblesAsync()
        {
            return await _dbSet
                .Where(a => a.EstaDisponible)
            .OrderBy(a => a.Codigo)
                .ToListAsync();
        }

        public async Task<Aula> GetByCodigoAsync(string codigo)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Codigo == codigo);
        }
    }
}
