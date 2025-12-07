using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Domain.ReadModels;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class ActividadExtracurricularRepository : GenericRepository<ActividadExtracurricular>, IActividadExtracurricularRepository
    {
        public ActividadExtracurricularRepository(ApplicationDbContext context) : base(context)
        {
        }

        
        public async Task<ActividadExtracurricular> GetActividadConDetallesAsync(int id)
        {
            return await _context.ActividadesExtracurriculares
                .Include(a => a.Inscripciones.Where(i => i.EstaActiva))
                    .ThenInclude(i => i.Estudiante)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<ActividadExtracurricular>> GetActividadesActivasAsync()
        {
            return await _context.ActividadesExtracurriculares
                .Where(a => a.EstaActiva)
                .Include(a => a.Inscripciones.Where(i => i.EstaActiva))
                .ToListAsync();
        }

        public async Task<List<ActividadExtracurricular>> GetActividadesPorCategoriaAsync(string categoria)
        {
            return await _context.ActividadesExtracurriculares
                .Where(a => a.Categoria == categoria && a.EstaActiva)
                .Include(a => a.Inscripciones.Where(i => i.EstaActiva))
                .ToListAsync();
        }

       
        public async Task<ActividadConDetalles> GetActividadDetalladaAsync(int id)
        {
            return await _context.ActividadesExtracurriculares
                .Where(a => a.Id == id)
                .Join(_context.Profesores,
                    a => a.IdProfesorEncargado,
                    p => p.Id,
                    (a, p) => new { a, p })
                .Join(_context.Users,
                    x => x.p.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new ActividadConDetalles
                    {
                        Id = x.a.Id,
                        Nombre = x.a.Nombre,
                        Descripcion = x.a.Descripcion,
                        Categoria = x.a.Categoria,
                        Requisitos = x.a.Requisitos,
                        HoraInicio = x.a.HoraInicio,
                        HoraFin = x.a.HoraFin,
                        DiaSemana = x.a.DiaSemana,
                        Ubicacion = x.a.Ubicacion,
                        ColorTarjeta = x.a.ColorTarjeta,
                        RutaImagen = x.a.RutaImagen,
                        EstaActiva = x.a.EstaActiva,
                        FechaCreacion = x.a.FechaCreacion,
                        IdProfesorEncargado = x.p.Id,
                        ProfesorNombre = u.FirstName,
                        ProfesorApellido = u.LastName,
                        TotalInscritos = _context.InscripcionesActividades
                            .Count(i => i.IdActividad == x.a.Id && i.EstaActiva)
                    })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ActividadConDetalles>> GetAllActividadesDetalladasAsync()
        {
            return await _context.ActividadesExtracurriculares
                .Join(_context.Profesores,
                    a => a.IdProfesorEncargado,
                    p => p.Id,
                    (a, p) => new { a, p })
                .Join(_context.Users,
                    x => x.p.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new ActividadConDetalles
                    {
                        Id = x.a.Id,
                        Nombre = x.a.Nombre,
                        Descripcion = x.a.Descripcion,
                        Categoria = x.a.Categoria,
                        Requisitos = x.a.Requisitos,
                        HoraInicio = x.a.HoraInicio,
                        HoraFin = x.a.HoraFin,
                        DiaSemana = x.a.DiaSemana,
                        Ubicacion = x.a.Ubicacion,
                        ColorTarjeta = x.a.ColorTarjeta,
                        RutaImagen = x.a.RutaImagen,
                        EstaActiva = x.a.EstaActiva,
                        FechaCreacion = x.a.FechaCreacion,
                        IdProfesorEncargado = x.p.Id,
                        ProfesorNombre = u.FirstName,
                        ProfesorApellido = u.LastName,
                        TotalInscritos = _context.InscripcionesActividades
                            .Count(i => i.IdActividad == x.a.Id && i.EstaActiva)
                    })
                .ToListAsync();
        }

        public async Task<List<ActividadConDetalles>> GetActividadesActivasDetalladasAsync()
        {
            return await _context.ActividadesExtracurriculares
                .Where(a => a.EstaActiva)
                .Join(_context.Profesores,
                    a => a.IdProfesorEncargado,
                    p => p.Id,
                    (a, p) => new { a, p })
                .Join(_context.Users,
                    x => x.p.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new ActividadConDetalles
                    {
                        Id = x.a.Id,
                        Nombre = x.a.Nombre,
                        Descripcion = x.a.Descripcion,
                        Categoria = x.a.Categoria,
                        Requisitos = x.a.Requisitos,
                        HoraInicio = x.a.HoraInicio,
                        HoraFin = x.a.HoraFin,
                        DiaSemana = x.a.DiaSemana,
                        Ubicacion = x.a.Ubicacion,
                        ColorTarjeta = x.a.ColorTarjeta,
                        RutaImagen = x.a.RutaImagen,
                        EstaActiva = x.a.EstaActiva,
                        FechaCreacion = x.a.FechaCreacion,
                        IdProfesorEncargado = x.p.Id,
                        ProfesorNombre = u.FirstName,
                        ProfesorApellido = u.LastName,
                        TotalInscritos = _context.InscripcionesActividades
                            .Count(i => i.IdActividad == x.a.Id && i.EstaActiva)
                    })
                .ToListAsync();
        }

        public async Task<List<ActividadConDetalles>> GetActividadesPorCategoriaDetalladasAsync(string categoria)
        {
            return await _context.ActividadesExtracurriculares
                .Where(a => a.Categoria == categoria && a.EstaActiva)
                .Join(_context.Profesores,
                    a => a.IdProfesorEncargado,
                    p => p.Id,
                    (a, p) => new { a, p })
                .Join(_context.Users,
                    x => x.p.ApplicationUserId,
                    u => u.Id,
                    (x, u) => new ActividadConDetalles
                    {
                        Id = x.a.Id,
                        Nombre = x.a.Nombre,
                        Descripcion = x.a.Descripcion,
                        Categoria = x.a.Categoria,
                        Requisitos = x.a.Requisitos,
                        HoraInicio = x.a.HoraInicio,
                        HoraFin = x.a.HoraFin,
                        DiaSemana = x.a.DiaSemana,
                        Ubicacion = x.a.Ubicacion,
                        ColorTarjeta = x.a.ColorTarjeta,
                        RutaImagen = x.a.RutaImagen,
                        EstaActiva = x.a.EstaActiva,
                        FechaCreacion = x.a.FechaCreacion,
                        IdProfesorEncargado = x.p.Id,
                        ProfesorNombre = u.FirstName,
                        ProfesorApellido = u.LastName,
                        TotalInscritos = _context.InscripcionesActividades
                            .Count(i => i.IdActividad == x.a.Id && i.EstaActiva)
                    })
                .ToListAsync();
        }
    }
}
