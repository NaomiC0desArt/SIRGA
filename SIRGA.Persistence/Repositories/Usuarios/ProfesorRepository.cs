using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
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
            return await _dbSet.FirstOrDefaultAsync(p => p.ApplicationUserId == applicationUserId);
        }
    }
}
