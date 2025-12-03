using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SIRGA.Persistence.Repositories
{
    public class ClaseProgramadaRepository : IClaseProgramadaRepositoryExtended
    {
        private readonly ApplicationDbContext _context;

        public ClaseProgramadaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClaseProgramada> AddAsync(ClaseProgramada claseProgramada)
        {
            await _context.ClasesProgramadas.AddAsync(claseProgramada);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(claseProgramada.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var claseProgramada = await GetByIdAsync(id);
            if (claseProgramada == null)
            {
                return false;
            }

            _context.ClasesProgramadas.Remove(claseProgramada);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ClaseProgramada>> GetAllAsync()
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .ToListAsync();
        }

        public async Task<ClaseProgramada> GetByIdAsync(int id)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ClaseProgramada> UpdateAsync(ClaseProgramada claseProgramada)
        {
            _context.ClasesProgramadas.Update(claseProgramada);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(claseProgramada.Id);
        }

        public async Task<List<ClaseProgramadaConDetalles>> GetAllWithDetailsAsync()
        {
            return await (from cp in _context.ClasesProgramadas
                          join asig in _context.Asignaturas on cp.IdAsignatura equals asig.Id
                          join prof in _context.Profesores on cp.IdProfesor equals prof.Id
                          join user in _context.Users on prof.ApplicationUserId equals user.Id
                          join curso in _context.CursosAcademicos on cp.IdCursoAcademico equals curso.Id
                          join grado in _context.Grados on curso.IdGrado equals grado.Id
                          select new ClaseProgramadaConDetalles
                          {
                              Id = cp.Id,
                              StartTime = cp.StartTime,
                              EndTime = cp.EndTime,
                              WeekDay = cp.WeekDay,
                              Location = cp.Location,
                              IdAsignatura = cp.IdAsignatura,
                              AsignaturaNombre = asig.Nombre,
                              IdProfesor = cp.IdProfesor,
                              ProfesorFirstName = user.FirstName,
                              ProfesorLastName = user.LastName,
                              IdCursoAcademico = cp.IdCursoAcademico,
                              GradoNombre = grado.GradeName,
                              GradoSeccion = grado.Section,
                              SchoolYear = curso.SchoolYear
                          }).ToListAsync();
        }

        public async Task<ClaseProgramadaConDetalles> GetByIdWithDetailsAsync(int id)
        {
            return await (from cp in _context.ClasesProgramadas
                          join asig in _context.Asignaturas on cp.IdAsignatura equals asig.Id
                          join prof in _context.Profesores on cp.IdProfesor equals prof.Id
                          join user in _context.Users on prof.ApplicationUserId equals user.Id
                          join curso in _context.CursosAcademicos on cp.IdCursoAcademico equals curso.Id
                          join grado in _context.Grados on curso.IdGrado equals grado.Id
                          where cp.Id == id
                          select new ClaseProgramadaConDetalles
                          {
                              Id = cp.Id,
                              StartTime = cp.StartTime,
                              EndTime = cp.EndTime,
                              WeekDay = cp.WeekDay,
                              Location = cp.Location,
                              IdAsignatura = cp.IdAsignatura,
                              AsignaturaNombre = asig.Nombre,
                              IdProfesor = cp.IdProfesor,
                              ProfesorFirstName = user.FirstName,
                              ProfesorLastName = user.LastName,
                              IdCursoAcademico = cp.IdCursoAcademico,
                              GradoNombre = grado.GradeName,
                              GradoSeccion = grado.Section,
                              SchoolYear = curso.SchoolYear
                          }).FirstOrDefaultAsync();
        }
        public async Task<List<ClaseProgramada>> GetClasesByProfesorAndDayAsync(int idProfesor, DayOfWeek diaSemana)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.Profesor)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .Where(c => c.IdProfesor == idProfesor && c.WeekDay == diaSemana)
                .OrderBy(c => c.StartTime)
                .ToListAsync();
        }

        // Implementar métodos faltantes de IBaseRepository
        public async Task<List<ClaseProgramada>> GetAllByConditionAsync(Expression<Func<ClaseProgramada, bool>> predicate)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.Profesor)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<ClaseProgramada> GetByConditionAsync(Expression<Func<ClaseProgramada, bool>> predicate)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.Profesor)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<ClaseProgramada, bool>> predicate)
        {
            return await _context.ClasesProgramadas.AnyAsync(predicate);
        }
    }
}
