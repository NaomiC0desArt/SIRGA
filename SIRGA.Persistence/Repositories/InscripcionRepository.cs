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
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Seccion)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.AnioEscolar)
                .FirstOrDefaultAsync(expression);
        }

        public async Task<List<Inscripcion>> GetAllByConditionAsync(Expression<Func<Inscripcion, bool>> expression)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Grado)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.Seccion)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(c => c.AnioEscolar)
                .Where(expression)
                .ToListAsync();
        }

        public async Task<InscripcionConDetalles> GetInscripcionConDetallesAsync(int id)
        {
            return await (from i in _context.Inscripciones
                          join e in _context.Estudiantes on i.IdEstudiante equals e.Id
                          join u in _context.Users on e.ApplicationUserId equals u.Id
                          join c in _context.CursosAcademicos on i.IdCursoAcademico equals c.Id
                          join g in _context.Grados on c.IdGrado equals g.Id
                          join s in _context.Secciones on c.IdSeccion equals s.Id
                          join a in _context.AniosEscolares on c.IdAnioEscolar equals a.Id
                          where i.Id == id
                          select new InscripcionConDetalles
                          {
                              Id = i.Id,
                              IdEstudiante = i.IdEstudiante,
                              IdCursoAcademico = i.IdCursoAcademico,
                              FechaInscripcion = i.FechaInscripcion,
                              EstudianteMatricula = e.Matricula,
                              EstudianteNombre = u.FirstName,
                              EstudianteApellido = u.LastName,
                              GradoNombre = g.GradeName,
                              SeccionNombre = s.Nombre,
                              AnioEscolarPeriodo = a.Periodo
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<InscripcionConDetalles>> GetAllInscripcionesConDetallesAsync()
        {
            return await (from i in _context.Inscripciones
                          join e in _context.Estudiantes on i.IdEstudiante equals e.Id
                          join u in _context.Users on e.ApplicationUserId equals u.Id
                          join c in _context.CursosAcademicos on i.IdCursoAcademico equals c.Id
                          join g in _context.Grados on c.IdGrado equals g.Id
                          join s in _context.Secciones on c.IdSeccion equals s.Id
                          join a in _context.AniosEscolares on c.IdAnioEscolar equals a.Id
                          select new InscripcionConDetalles
                          {
                              Id = i.Id,
                              IdEstudiante = i.IdEstudiante,
                              IdCursoAcademico = i.IdCursoAcademico,
                              FechaInscripcion = i.FechaInscripcion,
                              EstudianteMatricula = e.Matricula,
                              EstudianteNombre = u.FirstName,
                              EstudianteApellido = u.LastName,
                              GradoNombre = g.GradeName,
                              SeccionNombre = s.Nombre,
                              AnioEscolarPeriodo = a.Periodo
                          }).ToListAsync();
        }

        public async Task<Inscripcion> GetInscripcionActivaByEstudianteIdAsync(int estudianteId)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante) // ✅ Solo incluimos Estudiante
                .Include(i => i.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(ca => ca.Seccion)
                .Include(i => i.CursoAcademico)
                    .ThenInclude(ca => ca.AnioEscolar)
                .FirstOrDefaultAsync(i =>
                    i.IdEstudiante == estudianteId &&
                    i.Estado == "Activa");
        }

        public async Task<List<InscripcionConDetalles>> GetInscripcionesPorCursoAsync(int idCursoAcademico)
        {
            return await (from i in _context.Inscripciones
                          join e in _context.Estudiantes on i.IdEstudiante equals e.Id
                          join u in _context.Users on e.ApplicationUserId equals u.Id
                          join c in _context.CursosAcademicos on i.IdCursoAcademico equals c.Id
                          join g in _context.Grados on c.IdGrado equals g.Id
                          join s in _context.Secciones on c.IdSeccion equals s.Id
                          join a in _context.AniosEscolares on c.IdAnioEscolar equals a.Id
                          where i.IdCursoAcademico == idCursoAcademico
                          select new InscripcionConDetalles
                          {
                              Id = i.Id,
                              IdEstudiante = i.IdEstudiante,
                              IdCursoAcademico = i.IdCursoAcademico,
                              FechaInscripcion = i.FechaInscripcion,
                              EstudianteMatricula = e.Matricula,
                              EstudianteNombre = u.FirstName,
                              EstudianteApellido = u.LastName,
                              GradoNombre = g.GradeName,
                              SeccionNombre = s.Nombre,
                              AnioEscolarPeriodo = a.Periodo
                          }).ToListAsync();
        }
    }
}
