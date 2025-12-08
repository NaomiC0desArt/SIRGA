

using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.ReadModels;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;
using System.Linq.Expressions;

namespace SIRGA.Persistence.Repositories
{
    public class InscripcionActividadRepository : GenericRepository<InscripcionActividad>, IInscripcionActividadRepository
    {
        public InscripcionActividadRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> EstaInscritoAsync(int idEstudiante, int idActividad)
        {
            return await _context.InscripcionesActividades
                .AnyAsync(i => i.IdEstudiante == idEstudiante &&
                              i.IdActividad == idActividad &&
                              i.EstaActiva);
        }

        public async Task<InscripcionActividad> GetInscripcionActivaAsync(int idEstudiante, int idActividad)
        {
            return await _context.InscripcionesActividades
                .FirstOrDefaultAsync(i => i.IdEstudiante == idEstudiante &&
                                         i.IdActividad == idActividad &&
                                         i.EstaActiva);
        }

        public async Task<List<InscripcionActividad>> GetInscripcionesPorActividadAsync(int idActividad)
        {
            return await _context.InscripcionesActividades
                .Where(i => i.IdActividad == idActividad && i.EstaActiva)
                .Include(i => i.Estudiante)
                .Include(i => i.Actividad)
                .ToListAsync();
        }

        
        public async Task<List<InscripcionActividadConDetalles>> GetInscripcionesPorActividadConDetallesAsync(int idActividad)
        {
            return await _context.InscripcionesActividades
                .Where(i => i.IdActividad == idActividad && i.EstaActiva)
                .Join(_context.Estudiantes,
                    i => i.IdEstudiante,
                    e => e.Id,
                    (i, e) => new { i, e })
                .Join(_context.Users,
                    x => x.e.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.i, x.e, u })
                .Join(_context.ActividadesExtracurriculares,
                    x => x.i.IdActividad,
                    a => a.Id,
                    (x, a) => new InscripcionActividadConDetalles
                    {
                        Id = x.i.Id,
                        IdEstudiante = x.i.IdEstudiante,
                        IdActividad = x.i.IdActividad,
                        FechaInscripcion = x.i.FechaInscripcion,
                        EstaActiva = x.i.EstaActiva,
                        EstudianteMatricula = x.e.Matricula,
                        EstudianteNombre = x.u.FirstName,
                        EstudianteApellido = x.u.LastName,
                        ActividadNombre = a.Nombre,
                        ActividadCategoria = a.Categoria
                    })
                .ToListAsync();
        }

        public async Task<List<InscripcionActividadConDetalles>> GetInscripcionesPorEstudianteConDetallesAsync(int idEstudiante)
        {
            return await _context.InscripcionesActividades
                .Where(i => i.IdEstudiante == idEstudiante && i.EstaActiva)
                .Join(_context.Estudiantes,
                    i => i.IdEstudiante,
                    e => e.Id,
                    (i, e) => new { i, e })
                .Join(_context.Users,
                    x => x.e.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.i, x.e, u })
                .Join(_context.ActividadesExtracurriculares,
                    x => x.i.IdActividad,
                    a => a.Id,
                    (x, a) => new InscripcionActividadConDetalles
                    {
                        Id = x.i.Id,
                        IdEstudiante = x.i.IdEstudiante,
                        IdActividad = x.i.IdActividad,
                        FechaInscripcion = x.i.FechaInscripcion,
                        EstaActiva = x.i.EstaActiva,
                        EstudianteMatricula = x.e.Matricula,
                        EstudianteNombre = x.u.FirstName,
                        EstudianteApellido = x.u.LastName,
                        ActividadNombre = a.Nombre,
                        ActividadCategoria = a.Categoria
                    })
                .ToListAsync();
        }
    }
}
