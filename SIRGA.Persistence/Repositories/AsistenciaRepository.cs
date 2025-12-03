
using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class AsistenciaRepository : GenericRepository<Asistencia>, IAsistenciaRepository
    {
        public AsistenciaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Asistencia>> GetAsistenciasByClaseAndFechaAsync(int idClaseProgramada, DateTime fecha)
        {
            return await _dbSet
                .Include(a => a.Estudiante)
                .Include(a => a.ClaseProgramada)
                    .ThenInclude(c => c.Asignatura)
                .Where(a => a.IdClaseProgramada == idClaseProgramada &&
                            a.Fecha.Date == fecha.Date)
                .OrderBy(a => a.Estudiante.ApplicationUserId) // Ordenar alfabéticamente
                .ToListAsync();
        }

        public async Task<Asistencia?> GetAsistenciaByEstudianteClaseFechaAsync(
            int idEstudiante,
            int idClaseProgramada,
            DateTime fecha)
        {
            return await _dbSet
                .Include(a => a.Estudiante)
                .Include(a => a.ClaseProgramada)
                .FirstOrDefaultAsync(a =>
                    a.IdEstudiante == idEstudiante &&
                    a.IdClaseProgramada == idClaseProgramada &&
                    a.Fecha.Date == fecha.Date);
        }

        public async Task<List<Asistencia>> GetAsistenciasByEstudianteAsync(
            int idEstudiante,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            var query = _dbSet
                .Include(a => a.ClaseProgramada)
                    .ThenInclude(c => c.Asignatura)
                .Include(a => a.Profesor)
                .Where(a => a.IdEstudiante == idEstudiante);

            if (fechaInicio.HasValue)
                query = query.Where(a => a.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(a => a.Fecha <= fechaFin.Value);

            return await query
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<List<Asistencia>> GetAsistenciasByProfesorAsync(
            int idProfesor,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            var query = _dbSet
                .Include(a => a.Estudiante)
                .Include(a => a.ClaseProgramada)
                    .ThenInclude(c => c.Asignatura)
                .Where(a => a.IdProfesor == idProfesor);

            if (fechaInicio.HasValue)
                query = query.Where(a => a.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(a => a.Fecha <= fechaFin.Value);

            return await query
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<List<Asistencia>> GetAsistenciasRequierenJustificacionAsync()
        {
            return await _dbSet
                .Include(a => a.Estudiante)
                .Include(a => a.ClaseProgramada)
                    .ThenInclude(c => c.Asignatura)
                .Include(a => a.Profesor)
                .Where(a => a.RequiereJustificacion && string.IsNullOrEmpty(a.Justificacion))
                .OrderBy(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<bool> ExisteAsistenciaAsync(int idEstudiante, int idClaseProgramada, DateTime fecha)
        {
            return await _dbSet.AnyAsync(a =>
                a.IdEstudiante == idEstudiante &&
                a.IdClaseProgramada == idClaseProgramada &&
                a.Fecha.Date == fecha.Date);
        }

        public async Task<int> GetTotalAsistenciasByEstudianteAsync(int idEstudiante, string? estado = null)
        {
            var query = _dbSet.Where(a => a.IdEstudiante == idEstudiante);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(a => a.Estado == estado);

            return await query.CountAsync();
        }

        public async Task<List<Asistencia>> GetHistorialAsistenciasAsync(
            int idClaseProgramada,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            return await _dbSet
                .Include(a => a.Estudiante)
                .Include(a => a.ClaseProgramada)
                    .ThenInclude(c => c.Asignatura)
                .Where(a => a.IdClaseProgramada == idClaseProgramada &&
                            a.Fecha >= fechaInicio &&
                            a.Fecha <= fechaFin)
                .OrderBy(a => a.Fecha)
                .ThenBy(a => a.Estudiante.ApplicationUserId)
                .ToListAsync();
        }
    }
}
