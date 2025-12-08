using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.ReadModels;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;


namespace SIRGA.Persistence.Repositories.Usuarios
{
    public class ProfesorRepository : GenericRepository<Profesor>, IProfesorRepository
    {
        public ProfesorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Profesor> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Profesores
                .FirstOrDefaultAsync(p => p.ApplicationUserId == applicationUserId);
        }

        // obtener profesor con datos de usuario
        public async Task<ProfesorConUsuario> GetProfesorConUsuarioAsync(int id)
        {
            return await _context.Profesores
                .Where(p => p.Id == id)
                .Join(_context.Users,
                    p => p.ApplicationUserId,
                    u => u.Id,
                    (p, u) => new ProfesorConUsuario
                    {
                        Id = p.Id,
                        Specialty = p.Specialty,
                        ApplicationUserId = p.ApplicationUserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Photo = u.Photo
                    })
                .FirstOrDefaultAsync();
        }

        // obtener múltiples profesores con datos de usuario
        public async Task<Dictionary<int, ProfesorConUsuario>> GetProfesoresConUsuarioAsync(List<int> ids)
        {
            return await _context.Profesores
                .Where(p => ids.Contains(p.Id))
                .Join(_context.Users,
                    p => p.ApplicationUserId,
                    u => u.Id,
                    (p, u) => new ProfesorConUsuario
                    {
                        Id = p.Id,
                        Specialty = p.Specialty,
                        ApplicationUserId = p.ApplicationUserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Photo = u.Photo
                    })
                .ToDictionaryAsync(p => p.Id);
        }
    }
}
