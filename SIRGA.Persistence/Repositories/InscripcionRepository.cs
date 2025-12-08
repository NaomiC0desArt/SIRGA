using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.ReadModels;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;
using System.Linq.Expressions;

namespace SIRGA.Persistence.Repositories
{
    public class InscripcionRepository : GenericRepository<Inscripcion>, IInscripcionRepository
    
    {
        public InscripcionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Inscripcion> GetByConditionAsync(Expression<Func<Inscripcion, bool>> expression)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .FirstOrDefaultAsync(expression);
        }

        public async Task<List<Inscripcion>> GetAllByConditionAsync(Expression<Func<Inscripcion, bool>> expression)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .Where(expression)
                .ToListAsync();
        }

        // obtener inscripción con todos los detalles
        public async Task<InscripcionConDetalles> GetInscripcionConDetallesAsync(int id)
        {
            return await _context.Inscripciones
                .Where(i => i.Id == id)
                .Join(_context.Estudiantes,
                    i => i.IdEstudiante,
                    e => e.Id,
                    (i, e) => new { i, e })
                .Join(_context.Users,
                    x => x.e.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.i, x.e, u })
                .Join(_context.CursosAcademicos.Include(c => c.Grado),
                    x => x.i.IdCursoAcademico,
                    c => c.Id,
                    (x, c) => new InscripcionConDetalles
                    {
                        Id = x.i.Id,
                        IdEstudiante = x.i.IdEstudiante,
                        IdCursoAcademico = x.i.IdCursoAcademico,
                        FechaInscripcion = x.i.FechaInscripcion,
                        EstudianteMatricula = x.e.Matricula,
                        EstudianteNombre = x.u.FirstName,
                        EstudianteApellido = x.u.LastName,
                        GradoNombre = c.Grado.GradeName,
                        GradoSeccion = c.Grado.Section,
                        SchoolYear = c.SchoolYear
                    })
                .FirstOrDefaultAsync();
        }

        // obtener todas las inscripciones con detalles
        public async Task<List<InscripcionConDetalles>> GetAllInscripcionesConDetallesAsync()
        {
            return await _context.Inscripciones
                .Join(_context.Estudiantes,
                    i => i.IdEstudiante,
                    e => e.Id,
                    (i, e) => new { i, e })
                .Join(_context.Users,
                    x => x.e.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.i, x.e, u })
                .Join(_context.CursosAcademicos.Include(c => c.Grado),
                    x => x.i.IdCursoAcademico,
                    c => c.Id,
                    (x, c) => new InscripcionConDetalles
                    {
                        Id = x.i.Id,
                        IdEstudiante = x.i.IdEstudiante,
                        IdCursoAcademico = x.i.IdCursoAcademico,
                        FechaInscripcion = x.i.FechaInscripcion,
                        EstudianteMatricula = x.e.Matricula,
                        EstudianteNombre = x.u.FirstName,
                        EstudianteApellido = x.u.LastName,
                        GradoNombre = c.Grado.GradeName,
                        GradoSeccion = c.Grado.Section,
                        SchoolYear = c.SchoolYear
                    })
                .ToListAsync();
        }

        // obtener inscripciones por curso académico
        public async Task<List<InscripcionConDetalles>> GetInscripcionesPorCursoAsync(int idCursoAcademico)
        {
            return await _context.Inscripciones
                .Where(i => i.IdCursoAcademico == idCursoAcademico)
                .Join(_context.Estudiantes,
                    i => i.IdEstudiante,
                    e => e.Id,
                    (i, e) => new { i, e })
                .Join(_context.Users,
                    x => x.e.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.i, x.e, u })
                .Join(_context.CursosAcademicos.Include(c => c.Grado),
                    x => x.i.IdCursoAcademico,
                    c => c.Id,
                    (x, c) => new InscripcionConDetalles
                    {
                        Id = x.i.Id,
                        IdEstudiante = x.i.IdEstudiante,
                        IdCursoAcademico = x.i.IdCursoAcademico,
                        FechaInscripcion = x.i.FechaInscripcion,
                        EstudianteMatricula = x.e.Matricula,
                        EstudianteNombre = x.u.FirstName,
                        EstudianteApellido = x.u.LastName,
                        GradoNombre = c.Grado.GradeName,
                        GradoSeccion = c.Grado.Section,
                        SchoolYear = c.SchoolYear
                    })
                .ToListAsync();
        }
    }
}
