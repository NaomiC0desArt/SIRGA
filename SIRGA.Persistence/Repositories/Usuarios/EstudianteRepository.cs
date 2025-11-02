using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories.Usuarios
{
    public class EstudianteRepository : GenericRepository<Estudiante>, IEstudianteRepository
    {
        public EstudianteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Estudiante> GetByMatriculaAsync(string matricula)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Matricula == matricula);
        }

        public async Task<Estudiante> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);
        }

        public async Task<string> GetLastMatriculaAsync()
        {
            var lastEstudiante = await _dbSet
                .OrderByDescending(e => e.Matricula)
                .FirstOrDefaultAsync();

            return lastEstudiante?.Matricula;
        }

        public async Task<bool> ExistsByMatriculaAsync(string matricula)
        {
            return await _dbSet.AnyAsync(e => e.Matricula == matricula);
        }
    }
}
