using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Repositories
{
    public class PeriodoRepository : GenericRepository<Periodo>, IPeriodoRepository
    {
        public PeriodoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Periodo>> GetAllWithAnioEscolarAsync()
        {
            return await _dbSet
                .Include(p => p.AnioEscolar)
                .OrderBy(p => p.AnioEscolarId)
                .ThenBy(p => p.Numero)
                .ToListAsync();
        }

        public async Task<Periodo> GetByIdWithAnioEscolarAsync(int id)
        {
            return await _dbSet
                .Include(p => p.AnioEscolar)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Periodo>> GetByAnioEscolarAsync(int anioEscolarId)
        {
            return await _dbSet
                .Include(p => p.AnioEscolar)
                .Where(p => p.AnioEscolarId == anioEscolarId)
                .OrderBy(p => p.Numero)
                .ToListAsync();
        }

        public async Task<bool> ExistePeriodoAsync(int numero, int anioEscolarId)
        {
            return await _dbSet.AnyAsync(p =>
                p.Numero == numero &&
                p.AnioEscolarId == anioEscolarId);
        }

        public async Task<Periodo> GetPeriodoActivoAsync()
        {
            var hoy = DateTime.Today;
            return await _dbSet
                .Include(p => p.AnioEscolar)
                .Where(p => p.AnioEscolar.Activo &&
                           p.FechaInicio <= hoy &&
                           p.FechaFin >= hoy)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> TienePeriodoActivoAsync(int anioEscolarId)
        {
            var hoy = DateTime.Today;
            return await _dbSet.AnyAsync(p =>
                p.AnioEscolarId == anioEscolarId &&
                p.FechaInicio <= hoy &&
                p.FechaFin >= hoy);
        }

        public async Task<int> ContarPeriodosPorAnioAsync(int anioEscolarId)
        {
            return await _dbSet.CountAsync(p => p.AnioEscolarId == anioEscolarId);
        }
    }
}
