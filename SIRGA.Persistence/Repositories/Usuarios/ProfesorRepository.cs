using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;


namespace SIRGA.Persistence.Repositories.Usuarios
{
    public class ProfesorRepository : IProfesorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfesorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Profesor> AddAsync(Profesor profesor)
        {
            await _context.Profesores.AddAsync(profesor);
            await _context.SaveChangesAsync();
            return profesor;
        }

        public async Task<Profesor> GetByIdAsync(int id)
        {
            return await _context.Profesores.FindAsync(id);
        }

        public async Task<Profesor> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Profesores
                .FirstOrDefaultAsync(p => p.ApplicationUserId == applicationUserId);
        }

        public async Task<List<Profesor>> GetAllAsync()
        {
            return await _context.Profesores.ToListAsync();
        }

        public async Task<Profesor> UpdateAsync(Profesor profesor)
        {
            _context.Profesores.Update(profesor);
            await _context.SaveChangesAsync();
            return profesor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var profesor = await GetByIdAsync(id);
            if (profesor == null) return false;

            _context.Profesores.Remove(profesor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
