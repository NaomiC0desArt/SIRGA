using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.ReadModels;
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
            return await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Matricula == matricula);
        }

        public async Task<Estudiante> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);
        }

        public async Task<string> GetLastMatriculaAsync()
        {
            return await _context.Estudiantes
                .OrderByDescending(e => e.Matricula)
                .Select(e => e.Matricula)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByMatriculaAsync(string matricula)
        {
            return await _context.Estudiantes
                .AnyAsync(e => e.Matricula == matricula);
        }

        // obtener estudiante con datos de usuario
        public async Task<EstudianteConUsuario> GetEstudianteConUsuarioAsync(int id)
        {
            return await _context.Estudiantes
                .Where(e => e.Id == id)
                .Join(_context.Users,
                    e => e.ApplicationUserId,
                    u => u.Id,
                    (e, u) => new EstudianteConUsuario
                    {
                        Id = e.Id,
                        Matricula = e.Matricula,
                        MedicalConditions = e.MedicalConditions,
                        MedicalNote = e.MedicalNote,
                        EmergencyContactName = e.EmergencyContactName,
                        EmergencyContactPhone = e.EmergencyContactPhone,
                        ApplicationUserId = e.ApplicationUserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Photo = u.Photo
                    })
                .FirstOrDefaultAsync();
        }

        // obtener múltiples estudiantes con datos de usuario
        public async Task<List<EstudianteConUsuario>> GetEstudiantesConUsuarioAsync(List<int> ids)
        {
            return await _context.Estudiantes
                .Where(e => ids.Contains(e.Id))
                .Join(_context.Users,
                    e => e.ApplicationUserId,
                    u => u.Id,
                    (e, u) => new EstudianteConUsuario
                    {
                        Id = e.Id,
                        Matricula = e.Matricula,
                        MedicalConditions = e.MedicalConditions,
                        MedicalNote = e.MedicalNote,
                        EmergencyContactName = e.EmergencyContactName,
                        EmergencyContactPhone = e.EmergencyContactPhone,
                        ApplicationUserId = e.ApplicationUserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Photo = u.Photo
                    })
                .ToListAsync();
        }
    }
}
