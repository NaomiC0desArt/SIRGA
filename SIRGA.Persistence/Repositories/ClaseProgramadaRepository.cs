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
        public async Task<List<ClaseConDetallesParaHorario>> GetClasesPorCursoAcademicoAsync(int idCursoAcademico)
        {
            return await _context.ClasesProgramadas
                .Where(c => c.IdCursoAcademico == idCursoAcademico)
                .Join(_context.Asignaturas,
                    c => c.IdAsignatura,
                    a => a.Id,
                    (c, a) => new { c, a })
                .Join(_context.Profesores,
                    x => x.c.IdProfesor,
                    p => p.Id,
                    (x, p) => new { x.c, x.a, p })
                .Join(_context.Users,
                    x => x.p.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.c, x.a, x.p, u })
                .Join(_context.CursosAcademicos.Include(ca => ca.Grado),
                    x => x.c.IdCursoAcademico,
                    ca => ca.Id,
                    (x, ca) => new ClaseConDetallesParaHorario
                    {
                        Id = x.c.Id,
                        StartTime = x.c.StartTime,
                        EndTime = x.c.EndTime,
                        WeekDay = x.c.WeekDay,
                        Location = x.c.Location,
                        IdAsignatura = x.a.Id,
                        AsignaturaNombre = x.a.Nombre,
                        IdProfesor = x.p.Id,
                        ProfesorNombre = x.u.FirstName,
                        ProfesorApellido = x.u.LastName,
                        IdCursoAcademico = ca.Id,
                        GradoNombre = ca.Grado.GradeName,
                        GradoSeccion = ca.Grado.Section,
                        SchoolYear = ca.SchoolYear
                    })
                .ToListAsync();
        }

        public async Task<List<ClaseConDetallesParaHorario>> GetClasesPorCursoYDiaAsync(int idCursoAcademico, DayOfWeek dia)
        {
            return await _context.ClasesProgramadas
                .Where(c => c.IdCursoAcademico == idCursoAcademico && c.WeekDay == dia)
                .Join(_context.Asignaturas,
                    c => c.IdAsignatura,
                    a => a.Id,
                    (c, a) => new { c, a })
                .Join(_context.Profesores,
                    x => x.c.IdProfesor,
                    p => p.Id,
                    (x, p) => new { x.c, x.a, p })
                .Join(_context.Users,
                    x => x.p.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new { x.c, x.a, x.p, u })
                .Join(_context.CursosAcademicos.Include(ca => ca.Grado),
                    x => x.c.IdCursoAcademico,
                    ca => ca.Id,
                    (x, ca) => new ClaseConDetallesParaHorario
                    {
                        Id = x.c.Id,
                        StartTime = x.c.StartTime,
                        EndTime = x.c.EndTime,
                        WeekDay = x.c.WeekDay,
                        Location = x.c.Location,
                        IdAsignatura = x.a.Id,
                        AsignaturaNombre = x.a.Nombre,
                        IdProfesor = x.p.Id,
                        ProfesorNombre = x.u.FirstName,
                        ProfesorApellido = x.u.LastName,
                        IdCursoAcademico = ca.Id,
                        GradoNombre = ca.Grado.GradeName,
                        GradoSeccion = ca.Grado.Section,
                        SchoolYear = ca.SchoolYear
                    })
                .OrderBy(c => c.StartTime)
                .ToListAsync();
        }
    }
}
