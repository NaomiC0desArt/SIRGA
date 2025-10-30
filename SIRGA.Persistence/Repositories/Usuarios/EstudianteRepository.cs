using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Repositories.Usuarios
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly ApplicationDbContext _context;

        public EstudianteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Estudiante> AddAsync(Estudiante estudiante)
        {
            await _context.Estudiantes.AddAsync(estudiante);
            await _context.SaveChangesAsync();
            return estudiante;
        }

        public async Task<Estudiante> GetByIdAsync(int id)
        {
            return await _context.Estudiantes.FindAsync(id);
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

        public async Task<List<Estudiante>> GetAllAsync()
        {
            return await _context.Estudiantes.ToListAsync();
        }

        public async Task<Estudiante> UpdateAsync(Estudiante estudiante)
        {
            _context.Estudiantes.Update(estudiante);
            await _context.SaveChangesAsync();
            return estudiante;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var estudiante = await GetByIdAsync(id);
            if (estudiante == null) return false;

            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetLastMatriculaAsync()
        {
            var lastEstudiante = await _context.Estudiantes
                .OrderByDescending(e => e.Matricula)
                .FirstOrDefaultAsync();

            return lastEstudiante?.Matricula;
        }

        public async Task<bool> ExistsByMatriculaAsync(string matricula)
        {
            return await _context.Estudiantes
                .AnyAsync(e => e.Matricula == matricula);
        }
    }
}
