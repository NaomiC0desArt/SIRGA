using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.ActividadExtracurricular;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Services
{
    public class ActividadExtracurricularService : IActividadExtracurricularService
    {
        private readonly IActividadExtracurricularRepository _actividadRepository;
        private readonly IInscripcionActividadRepository _inscripcionRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActividadExtracurricularService> _logger;
        private readonly string _uploadsPath;

        public ActividadExtracurricularService(
            IActividadExtracurricularRepository actividadRepository,
            IInscripcionActividadRepository inscripcionRepository,
            ApplicationDbContext context,
            ILogger<ActividadExtracurricularService> logger)
        {
            _actividadRepository = actividadRepository;
            _inscripcionRepository = inscripcionRepository;
            _context = context;
            _logger = logger;
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "actividades");

            // Crear directorio si no existe
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        public async Task<ApiResponse<ActividadExtracurricularDto>> CreateAsync(CreateActividadDto dto, IFormFile imagen = null)
        {
            try
            {
                _logger.LogInformation("=== INICIANDO CREACION DE ACTIVIDAD EN EL SERVICIO ===");
                _logger.LogInformation("Datos recibidos: {@Dto}", dto);

                // Validar horario (2:00 PM - 8:00 PM)
                if (dto.HoraInicio < TimeSpan.FromHours(14) || dto.HoraFin > TimeSpan.FromHours(20))
                {
                    _logger.LogWarning("Horario inválido: {HoraInicio} - {HoraFin}", dto.HoraInicio, dto.HoraFin);
                    return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                        "El horario debe estar entre las 2:00 PM y las 8:00 PM");
                }

                if (dto.HoraFin <= dto.HoraInicio)
                {
                    _logger.LogWarning("Hora fin menor o igual a hora inicio");
                    return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                        "La hora de fin debe ser posterior a la hora de inicio");
                }

                // Verificar que el profesor existe
                _logger.LogInformation("Verificando existencia del profesor ID: {IdProfesor}", dto.IdProfesorEncargado);
                var profesorExists = await _context.Profesores.AnyAsync(p => p.Id == dto.IdProfesorEncargado);

                if (!profesorExists)
                {
                    _logger.LogError("Profesor no encontrado con ID: {IdProfesor}", dto.IdProfesorEncargado);
                    return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                        $"El profesor con ID {dto.IdProfesorEncargado} no existe");
                }

                _logger.LogInformation("✓ Profesor encontrado");

                string rutaImagen = null;
                if (imagen != null)
                {
                    _logger.LogInformation("Guardando imagen: {FileName}, Tamaño: {Size}", imagen.FileName, imagen.Length);
                    rutaImagen = await GuardarImagenAsync(imagen);
                    _logger.LogInformation("✓ Imagen guardada en: {RutaImagen}", rutaImagen);
                }

                var actividad = new ActividadExtracurricular
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Categoria = dto.Categoria,
                    Requisitos = dto.Requisitos,
                    HoraInicio = dto.HoraInicio,
                    HoraFin = dto.HoraFin,
                    DiaSemana = ConvertirDiaADayOfWeek(dto.DiaSemana),
                    Ubicacion = dto.Ubicacion,
                    ColorTarjeta = dto.ColorTarjeta,
                    RutaImagen = rutaImagen,
                    IdProfesorEncargado = dto.IdProfesorEncargado,
                    EstaActiva = true,
                    FechaCreacion = DateTime.Now
                };

                _logger.LogInformation("Objeto actividad creado: {@Actividad}", new
                {
                    actividad.Nombre,
                    actividad.Categoria,
                    actividad.IdProfesorEncargado,
                    actividad.DiaSemana,
                    actividad.HoraInicio,
                    actividad.HoraFin
                });

                _logger.LogInformation("Intentando guardar en la base de datos...");

                try
                {
                    await _actividadRepository.AddAsync(actividad);
                    _logger.LogInformation("✓✓✓ Actividad guardada exitosamente con ID: {Id}", actividad.Id);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "!!! ERROR AL GUARDAR EN LA BASE DE DATOS !!!");
                    _logger.LogError("Tipo de excepción: {Type}", dbEx.GetType().Name);
                    _logger.LogError("Mensaje: {Message}", dbEx.Message);

                    if (dbEx.InnerException != null)
                    {
                        _logger.LogError("Inner Exception: {InnerMessage}", dbEx.InnerException.Message);
                        _logger.LogError("Inner Exception Type: {InnerType}", dbEx.InnerException.GetType().Name);

                        if (dbEx.InnerException.InnerException != null)
                        {
                            _logger.LogError("Inner Inner Exception: {InnerInnerMessage}",
                                dbEx.InnerException.InnerException.Message);
                        }
                    }

                    throw; // Re-lanzar para que sea capturado por el catch externo
                }

                var actividadDto = await MapToDto(actividad);
                return ApiResponse<ActividadExtracurricularDto>.SuccessResponse(
                    actividadDto,
                    "Actividad creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR GENERAL al crear actividad extracurricular");
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);

                // Mensaje más específico para el usuario
                string userMessage = "Error al crear la actividad";
                List<string> errors = new List<string> { ex.Message };

                if (ex.InnerException != null)
                {
                    errors.Add(ex.InnerException.Message);

                    // Mensajes más amigables para errores comunes
                    if (ex.InnerException.Message.Contains("FOREIGN KEY"))
                    {
                        userMessage = "Error de referencia: El profesor seleccionado no es válido";
                        errors = new List<string> { "El profesor seleccionado no existe en el sistema" };
                    }
                    else if (ex.InnerException.Message.Contains("UNIQUE"))
                    {
                        userMessage = "Ya existe una actividad con ese nombre";
                        errors = new List<string> { "El nombre de la actividad ya está en uso" };
                    }
                }

                return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                    userMessage,
                    errors);
            }
        }

        public async Task<ApiResponse<ActividadExtracurricularDto>> UpdateAsync(int id, UpdateActividadDto dto, IFormFile imagen = null)
        {
            try
            {
                var actividad = await _actividadRepository.GetByIdAsync(id);
                if (actividad == null)
                {
                    return ApiResponse<ActividadExtracurricularDto>.ErrorResponse("Actividad no encontrada");
                }

                // Validar horario
                if (dto.HoraInicio < TimeSpan.FromHours(14) || dto.HoraFin > TimeSpan.FromHours(20))
                {
                    return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                        "El horario debe estar entre las 2:00 PM y las 8:00 PM");
                }

                if (dto.HoraFin <= dto.HoraInicio)
                {
                    return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                        "La hora de fin debe ser posterior a la hora de inicio");
                }

                // Actualizar imagen si se proporciona una nueva
                if (imagen != null)
                {
                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(actividad.RutaImagen))
                    {
                        EliminarImagen(actividad.RutaImagen);
                    }
                    actividad.RutaImagen = await GuardarImagenAsync(imagen);
                }

                actividad.Nombre = dto.Nombre;
                actividad.Descripcion = dto.Descripcion;
                actividad.Categoria = dto.Categoria;
                actividad.Requisitos = dto.Requisitos;
                actividad.HoraInicio = dto.HoraInicio;
                actividad.HoraFin = dto.HoraFin;
                actividad.DiaSemana = ConvertirDiaADayOfWeek(dto.DiaSemana);
                actividad.Ubicacion = dto.Ubicacion;
                actividad.ColorTarjeta = dto.ColorTarjeta;
                actividad.IdProfesorEncargado = dto.IdProfesorEncargado;
                actividad.EstaActiva = dto.EstaActiva;

                await _actividadRepository.UpdateAsync(actividad);

                var actividadDto = await MapToDto(actividad);
                return ApiResponse<ActividadExtracurricularDto>.SuccessResponse(
                    actividadDto,
                    "Actividad actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar actividad {id}");
                return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                    "Error al actualizar la actividad",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var actividad = await _actividadRepository.GetByIdAsync(id);
                if (actividad == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Actividad no encontrada");
                }

                // Eliminar imagen si existe
                if (!string.IsNullOrEmpty(actividad.RutaImagen))
                {
                    EliminarImagen(actividad.RutaImagen);
                }

                await _actividadRepository.DeleteAsync(id);
                return ApiResponse<bool>.SuccessResponse(true, "Actividad eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar actividad {id}");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la actividad",
                    new List<string> { ex.Message });
            }
        }

        // Continuación en Parte 2...
        public async Task<ApiResponse<ActividadDetalleDto>> GetByIdAsync(int id, int? estudianteId = null)
        {
            try
            {
                var actividad = await _actividadRepository.GetActividadConDetallesAsync(id);
                if (actividad == null)
                {
                    return ApiResponse<ActividadDetalleDto>.ErrorResponse("Actividad no encontrada");
                }

                var profesor = await _context.Profesores
                    .FirstOrDefaultAsync(p => p.Id == actividad.IdProfesorEncargado);

                var profesorUser = profesor != null
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == profesor.ApplicationUserId)
                    : null;

                var estudiantesInscritos = await ObtenerEstudiantesInscritos(id);

                bool estaInscrito = false;
                if (estudianteId.HasValue)
                {
                    estaInscrito = await _inscripcionRepository.EstaInscritoAsync(estudianteId.Value, id);
                }

                var detalleDto = new ActividadDetalleDto
                {
                    Id = actividad.Id,
                    Nombre = actividad.Nombre,
                    Descripcion = actividad.Descripcion,
                    Categoria = actividad.Categoria,
                    Requisitos = actividad.Requisitos,
                    HoraInicio = actividad.HoraInicio,
                    HoraFin = actividad.HoraFin,
                    DiaSemana = ConvertirDayOfWeekADia(actividad.DiaSemana),
                    Ubicacion = actividad.Ubicacion,
                    ColorTarjeta = actividad.ColorTarjeta,
                    RutaImagen = actividad.RutaImagen,
                    EstaActiva = actividad.EstaActiva,
                    NombreProfesor = profesorUser != null
                        ? $"{profesorUser.FirstName} {profesorUser.LastName}"
                        : "N/A",
                    TotalInscritos = estudiantesInscritos.Count,
                    EstudiantesInscritos = estudiantesInscritos,
                    EstaInscrito = estaInscrito
                };

                return ApiResponse<ActividadDetalleDto>.SuccessResponse(detalleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener actividad {id}");
                return ApiResponse<ActividadDetalleDto>.ErrorResponse(
                    "Error al obtener la actividad",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<ActividadExtracurricularDto>>> GetAllAsync(int? estudianteId = null)
        {
            try
            {
                var actividades = await _actividadRepository.GetAllAsync();
                var actividadesDto = new List<ActividadExtracurricularDto>();

                foreach (var actividad in actividades)
                {
                    var dto = await MapToDto(actividad, estudianteId);
                    actividadesDto.Add(dto);
                }

                return ApiResponse<List<ActividadExtracurricularDto>>.SuccessResponse(actividadesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades");
                return ApiResponse<List<ActividadExtracurricularDto>>.ErrorResponse(
                    "Error al obtener las actividades",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<ActividadExtracurricularDto>>> GetActividadesActivasAsync(int? estudianteId = null)
        {
            try
            {
                var actividades = await _actividadRepository.GetActividadesActivasAsync();
                var actividadesDto = new List<ActividadExtracurricularDto>();

                foreach (var actividad in actividades)
                {
                    var dto = await MapToDto(actividad, estudianteId);
                    actividadesDto.Add(dto);
                }

                return ApiResponse<List<ActividadExtracurricularDto>>.SuccessResponse(actividadesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades activas");
                return ApiResponse<List<ActividadExtracurricularDto>>.ErrorResponse(
                    "Error al obtener las actividades",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<ActividadExtracurricularDto>>> GetPorCategoriaAsync(string categoria, int? estudianteId = null)
        {
            try
            {
                var actividades = await _actividadRepository.GetActividadesPorCategoriaAsync(categoria);
                var actividadesDto = new List<ActividadExtracurricularDto>();

                foreach (var actividad in actividades)
                {
                    var dto = await MapToDto(actividad, estudianteId);
                    actividadesDto.Add(dto);
                }

                return ApiResponse<List<ActividadExtracurricularDto>>.SuccessResponse(actividadesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener actividades de categoría {categoria}");
                return ApiResponse<List<ActividadExtracurricularDto>>.ErrorResponse(
                    "Error al obtener las actividades",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<EstudianteInscritoDto>>> GetEstudiantesInscritosAsync(int idActividad)
        {
            try
            {
                var inscripciones = await _inscripcionRepository.GetInscripcionesPorActividadAsync(idActividad);
                var estudiantesDto = new List<EstudianteInscritoDto>();

                foreach (var inscripcion in inscripciones)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == inscripcion.Estudiante.ApplicationUserId);

                    estudiantesDto.Add(new EstudianteInscritoDto
                    {
                        IdEstudiante = inscripcion.IdEstudiante,
                        NombreCompleto = user != null ? $"{user.FirstName} {user.LastName}" : "N/A",
                        Matricula = inscripcion.Estudiante.Matricula,
                        FechaInscripcion = inscripcion.FechaInscripcion
                    });
                }

                return ApiResponse<List<EstudianteInscritoDto>>.SuccessResponse(estudiantesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener estudiantes inscritos en actividad {idActividad}");
                return ApiResponse<List<EstudianteInscritoDto>>.ErrorResponse(
                    "Error al obtener los estudiantes inscritos",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> InscribirEstudianteAsync(int idActividad, int idEstudiante)
        {
            try
            {
                var yaInscrito = await _inscripcionRepository.EstaInscritoAsync(idEstudiante, idActividad);
                if (yaInscrito)
                {
                    return ApiResponse<bool>.ErrorResponse("El estudiante ya está inscrito en esta actividad");
                }

                var inscripcion = new InscripcionActividad
                {
                    IdEstudiante = idEstudiante,
                    IdActividad = idActividad,
                    FechaInscripcion = DateTime.Now,
                    EstaActiva = true
                };

                await _inscripcionRepository.AddAsync(inscripcion);
                return ApiResponse<bool>.SuccessResponse(true, "Estudiante inscrito exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al inscribir estudiante {idEstudiante} en actividad {idActividad}");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al inscribir al estudiante",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DesinscribirEstudianteAsync(int idActividad, int idEstudiante)
        {
            try
            {
                var inscripcion = await _inscripcionRepository.GetInscripcionActivaAsync(idEstudiante, idActividad);
                if (inscripcion == null)
                {
                    return ApiResponse<bool>.ErrorResponse("No se encontró la inscripción");
                }

                inscripcion.EstaActiva = false;
                await _inscripcionRepository.UpdateAsync(inscripcion);

                return ApiResponse<bool>.SuccessResponse(true, "Estudiante desinscrito exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al desinscribir estudiante {idEstudiante} de actividad {idActividad}");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al desinscribir al estudiante",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> InscribirseAsync(int idActividad, int idEstudiante)
        {
            return await InscribirEstudianteAsync(idActividad, idEstudiante);
        }

        public async Task<ApiResponse<bool>> DesinscribirseAsync(int idActividad, int idEstudiante)
        {
            return await DesinscribirEstudianteAsync(idActividad, idEstudiante);
        }

        public async Task<ApiResponse<bool>> EstaInscritoAsync(int idActividad, int idEstudiante)
        {
            try
            {
                var inscrito = await _inscripcionRepository.EstaInscritoAsync(idEstudiante, idActividad);
                return ApiResponse<bool>.SuccessResponse(inscrito);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar inscripción de estudiante {idEstudiante} en actividad {idActividad}");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al verificar la inscripción",
                    new List<string> { ex.Message });
            }
        }

        private async Task<ActividadExtracurricularDto> MapToDto(ActividadExtracurricular actividad, int? estudianteId = null)
        {
            var profesor = await _context.Profesores
                .FirstOrDefaultAsync(p => p.Id == actividad.IdProfesorEncargado);

            var profesorUser = profesor != null
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == profesor.ApplicationUserId)
                : null;

            var totalInscritos = actividad.Inscripciones?.Count(i => i.EstaActiva) ?? 0;

            bool estaInscrito = false;
            if (estudianteId.HasValue)
            {
                estaInscrito = await _inscripcionRepository.EstaInscritoAsync(estudianteId.Value, actividad.Id);
            }

            return new ActividadExtracurricularDto
            {
                Id = actividad.Id,
                Nombre = actividad.Nombre,
                Descripcion = actividad.Descripcion,
                Categoria = actividad.Categoria,
                Requisitos = actividad.Requisitos,
                HoraInicio = actividad.HoraInicio,
                HoraFin = actividad.HoraFin,
                DiaSemana = ConvertirDayOfWeekADia(actividad.DiaSemana),
                Ubicacion = actividad.Ubicacion,
                ColorTarjeta = actividad.ColorTarjeta,
                RutaImagen = actividad.RutaImagen,
                EstaActiva = actividad.EstaActiva,
                IdProfesorEncargado = actividad.IdProfesorEncargado,
                NombreProfesor = profesorUser != null
                    ? $"{profesorUser.FirstName} {profesorUser.LastName}"
                    : "N/A",
                TotalInscritos = totalInscritos,
                EstaInscrito = estaInscrito
            };
        }

        private async Task<List<EstudianteInscritoDto>> ObtenerEstudiantesInscritos(int idActividad)
        {
            var inscripciones = await _inscripcionRepository.GetInscripcionesPorActividadAsync(idActividad);
            var estudiantesDto = new List<EstudianteInscritoDto>();

            foreach (var inscripcion in inscripciones)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == inscripcion.Estudiante.ApplicationUserId);

                estudiantesDto.Add(new EstudianteInscritoDto
                {
                    IdEstudiante = inscripcion.IdEstudiante,
                    NombreCompleto = user != null ? $"{user.FirstName} {user.LastName}" : "N/A",
                    Matricula = inscripcion.Estudiante.Matricula,
                    FechaInscripcion = inscripcion.FechaInscripcion
                });
            }

            return estudiantesDto;
        }
        private DayOfWeek ConvertirDiaADayOfWeek(string dia)
        {
            return dia switch
            {
                "Lunes" => DayOfWeek.Monday,
                "Martes" => DayOfWeek.Tuesday,
                "Miércoles" => DayOfWeek.Wednesday,
                "Jueves" => DayOfWeek.Thursday,
                "Viernes" => DayOfWeek.Friday,
                "Sábado" => DayOfWeek.Saturday,
                "Domingo" => DayOfWeek.Sunday,
                _ => throw new ArgumentException($"Día no válido: {dia}")
            };
        }

        private string ConvertirDayOfWeekADia(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => "Desconocido"
            };
        }

        private async Task<string> GuardarImagenAsync(IFormFile imagen)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return $"/uploads/actividades/{fileName}";
        }

        private void EliminarImagen(string rutaImagen)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rutaImagen.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"No se pudo eliminar la imagen: {rutaImagen}");
            }
        }
    }
}
