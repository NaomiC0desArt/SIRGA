using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Persistence.Repositories
{
    public class GradoRepository : IGradoRepository
    {
        private readonly ApplicationDbContext _context;

        public GradoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Grado> AddAsync(Grado grado)
        {
            await _context.Grados.AddAsync(grado);
            await _context.SaveChangesAsync();
            return grado;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var grado = await GetByIdAsync(id);
            if (grado == null) { return false; }

            _context.Grados.Remove(grado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Grado>> GetAllAsync()
        {
            return await _context.Grados.ToListAsync();
        }

        public async Task<Grado> GetByIdAsync(int id)
        {
            return await _context.Grados.FindAsync(id);
        }

        public async Task<Grado> UpdateAsync(Grado grado)
        {
            _context.Grados.Update(grado);
            await _context.SaveChangesAsync();
            return grado;
        }
    }
}
