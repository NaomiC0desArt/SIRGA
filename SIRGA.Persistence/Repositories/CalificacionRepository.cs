using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class CalificacionRepository : GenericRepository<Calificacion>, ICalificacionRepository
    {
        public CalificacionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Calificacion> GetCalificacionConDetallesAsync(int idEstudiante, int idAsignatura, int idPeriodo)
        {
            return await _context.Calificaciones
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Componente)
                .Include(c => c.Estudiante)
                .Include(c => c.Asignatura)
                .Include(c => c.Periodo)
                .FirstOrDefaultAsync(c =>
                    c.IdEstudiante == idEstudiante &&
                    c.IdAsignatura == idAsignatura &&
                c.IdPeriodo == idPeriodo);
        }

        public async Task<List<Calificacion>> GetCalificacionesPorProfesorYPeriodoAsync(int idProfesor, int idPeriodo)
        {
            return await _context.Calificaciones
                .Include(c => c.Estudiante)
                .Include(c => c.Asignatura)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Seccion)
                .Include(c => c.Periodo)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Componente)
                .Where(c => c.IdProfesor == idProfesor && c.IdPeriodo == idPeriodo)
                .ToListAsync();
        }

        public async Task<List<Calificacion>> GetCalificacionesPorEstudianteAsync(int idEstudiante)
        {
            return await _context.Calificaciones
                .Include(c => c.Asignatura)
                .Include(c => c.Periodo)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.AnioEscolar)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Componente)
                .Where(c => c.IdEstudiante == idEstudiante && c.Publicada)
                .OrderBy(c => c.Periodo.Numero)
                .ToListAsync();
        }

        public async Task<List<Calificacion>> GetCalificacionesPorCursoYAsignaturaAsync(
            int idCurso, int idAsignatura, int idPeriodo)
        {
            return await _context.Calificaciones
                .Include(c => c.Estudiante)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Componente)
                .Where(c =>
                    c.IdCursoAcademico == idCurso &&
                    c.IdAsignatura == idAsignatura &&
                    c.IdPeriodo == idPeriodo)
                .ToListAsync();
        }

        public async Task<bool> ExisteCalificacionAsync(int idEstudiante, int idAsignatura, int idPeriodo)
        {
            return await _context.Calificaciones
                .AnyAsync(c =>
                    c.IdEstudiante == idEstudiante &&
                    c.IdAsignatura == idAsignatura &&
                    c.IdPeriodo == idPeriodo);
        }

        public async Task<int> ContarCalificacionesPublicadasAsync(int idProfesor, int idPeriodo)
        {
            return await _context.Calificaciones
                .CountAsync(c => c.IdProfesor == idProfesor && c.IdPeriodo == idPeriodo && c.Publicada);
        }

        public async Task<int> ContarCalificacionesPendientesAsync(int idProfesor, int idPeriodo)
        {
            return await _context.Calificaciones
                .CountAsync(c => c.IdProfesor == idProfesor && c.IdPeriodo == idPeriodo && !c.Publicada);
        }
    }
}
