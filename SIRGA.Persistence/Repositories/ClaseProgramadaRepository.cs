using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.ReadModels;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class ClaseProgramadaRepository : GenericRepository<ClaseProgramada>, IClaseProgramadaRepository
    {
        private readonly ApplicationDbContext _context;

        public ClaseProgramadaRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<ClaseProgramadaConDetalles>> GetAllWithDetailsAsync()
        {
            return await (from cp in _context.ClasesProgramadas
                          join asig in _context.Asignaturas on cp.IdAsignatura equals asig.Id
                          join prof in _context.Profesores on cp.IdProfesor equals prof.Id
                          join user in _context.Users on prof.ApplicationUserId equals user.Id
                          join curso in _context.CursosAcademicos on cp.IdCursoAcademico equals curso.Id
                          join grado in _context.Grados on curso.IdGrado equals grado.Id
                          join seccion in _context.Secciones on curso.IdSeccion equals seccion.Id
                          join anio in _context.AniosEscolares on curso.IdAnioEscolar equals anio.Id
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
                              SeccionNombre = seccion.Nombre,
                              AnioEscolarPeriodo = anio.Periodo
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
                          join seccion in _context.Secciones on curso.IdSeccion equals seccion.Id
                          join anio in _context.AniosEscolares on curso.IdAnioEscolar equals anio.Id
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
                              SeccionNombre = seccion.Nombre,
                              AnioEscolarPeriodo = anio.Periodo
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<ClaseProgramada>> GetClasesByProfesorAndDayAsync(int idProfesor, DayOfWeek diaSemana)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.Profesor)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Seccion)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.AnioEscolar)
                .Where(c => c.IdProfesor == idProfesor && c.WeekDay == diaSemana)
                .OrderBy(c => c.StartTime)
                .ToListAsync();
        }

        public async Task<List<ClaseConDetallesParaHorario>> GetClasesPorCursoAcademicoAsync(int idCursoAcademico)
        {
            return await (from cp in _context.ClasesProgramadas
                          join asig in _context.Asignaturas on cp.IdAsignatura equals asig.Id
                          join prof in _context.Profesores on cp.IdProfesor equals prof.Id
                          join user in _context.Users on prof.ApplicationUserId equals user.Id
                          join curso in _context.CursosAcademicos on cp.IdCursoAcademico equals curso.Id
                          join grado in _context.Grados on curso.IdGrado equals grado.Id
                          join seccion in _context.Secciones on curso.IdSeccion equals seccion.Id
                          join anio in _context.AniosEscolares on curso.IdAnioEscolar equals anio.Id
                          where cp.IdCursoAcademico == idCursoAcademico
                          select new ClaseConDetallesParaHorario
                          {
                              Id = cp.Id,
                              StartTime = cp.StartTime,
                              EndTime = cp.EndTime,
                              WeekDay = cp.WeekDay,
                              Location = cp.Location,
                              IdAsignatura = asig.Id,
                              AsignaturaNombre = asig.Nombre,
                              IdProfesor = prof.Id,
                              ProfesorNombre = user.FirstName,
                              ProfesorApellido = user.LastName,
                              IdCursoAcademico = curso.Id,
                              GradoNombre = grado.GradeName,
                              SeccionNombre = seccion.Nombre,
                              AnioEscolarPeriodo = anio.Periodo
                          }).ToListAsync();
        }

        public async Task<List<ClaseConDetallesParaHorario>> GetClasesPorCursoYDiaAsync(int idCursoAcademico, DayOfWeek dia)
        {
            return await (from cp in _context.ClasesProgramadas
                          join asig in _context.Asignaturas on cp.IdAsignatura equals asig.Id
                          join prof in _context.Profesores on cp.IdProfesor equals prof.Id
                          join user in _context.Users on prof.ApplicationUserId equals user.Id
                          join curso in _context.CursosAcademicos on cp.IdCursoAcademico equals curso.Id
                          join grado in _context.Grados on curso.IdGrado equals grado.Id
                          join seccion in _context.Secciones on curso.IdSeccion equals seccion.Id
                          join anio in _context.AniosEscolares on curso.IdAnioEscolar equals anio.Id
                          where cp.IdCursoAcademico == idCursoAcademico && cp.WeekDay == dia
                          select new ClaseConDetallesParaHorario
                          {
                              Id = cp.Id,
                              StartTime = cp.StartTime,
                              EndTime = cp.EndTime,
                              WeekDay = cp.WeekDay,
                              Location = cp.Location,
                              IdAsignatura = asig.Id,
                              AsignaturaNombre = asig.Nombre,
                              IdProfesor = prof.Id,
                              ProfesorNombre = user.FirstName,
                              ProfesorApellido = user.LastName,
                              IdCursoAcademico = curso.Id,
                              GradoNombre = grado.GradeName,
                              SeccionNombre = seccion.Nombre,
                              AnioEscolarPeriodo = anio.Periodo
                          })
                          .OrderBy(c => c.StartTime)
                          .ToListAsync();
        }

        public async Task<List<ClaseProgramada>> GetByProfesorAsync(int idProfesor)
        {
            return await _context.ClasesProgramadas
                .Include(c => c.Asignatura)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.Seccion)
                .Include(c => c.CursoAcademico)
                    .ThenInclude(ca => ca.AnioEscolar)
                .Where(c => c.IdProfesor == idProfesor)
                .ToListAsync();
        }
    }
}