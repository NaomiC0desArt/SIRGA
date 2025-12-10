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
    public class AnioEscolarRepository : GenericRepository<AnioEscolar>, IAnioEscolarRepository
    {
        public AnioEscolarRepository(ApplicationDbContext context) : base(context) { }

        public async Task<AnioEscolar> GetAnioActivoAsync()
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Activo);
        }

        public async Task<bool> ExisteAnioActivoAsync()
        {
            return await _dbSet.AnyAsync(a => a.Activo);
        }

        public async Task DesactivarTodosAsync()
        {
            var aniosActivos = await _dbSet.Where(a => a.Activo).ToListAsync();
            foreach (var anio in aniosActivos)
            {
                anio.Activo = false;
            }
            await _context.SaveChangesAsync();
        }
    }
}
