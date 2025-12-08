using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.ActividadExtracurricular;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
namespace SIRGA.Application.Services
{
    public class ActividadExtracurricularService : IActividadExtracurricularService
    {
        private readonly IActividadExtracurricularRepository _actividadRepository;
        private readonly IInscripcionActividadRepository _inscripcionRepository;
        private readonly ILogger<ActividadExtracurricularService> _logger;
        private readonly string _uploadsPath;

        public ActividadExtracurricularService(
            IActividadExtracurricularRepository actividadRepository,
            IInscripcionActividadRepository inscripcionRepository,
            ILogger<ActividadExtracurricularService> logger)
        {
            _actividadRepository = actividadRepository;
            _inscripcionRepository = inscripcionRepository;
            _logger = logger;
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "actividades");

            // Crear directorio si no existe
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        #region CRUD Operations
        public async Task<ApiResponse<ActividadExtracurricularDto>> CreateAsync(CreateActividadDto dto, IFormFile imagen = null)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de actividad: {Nombre}", dto.Nombre);

                // Validar horario (2:00 PM - 8:00 PM)
                var validacionHorario = ValidarHorario(dto.HoraInicio, dto.HoraFin);
                if (validacionHorario != null)
                    return validacionHorario;

                // Guardar imagen si existe
                string rutaImagen = null;
                if (imagen != null)
                {
                    _logger.LogInformation("Guardando imagen: {FileName}", imagen.FileName);
                    rutaImagen = await GuardarImagenAsync(imagen);
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

                await _actividadRepository.AddAsync(actividad);
                _logger.LogInformation("✓ Actividad creada con ID: {Id}", actividad.Id);

                // Obtener actividad con detalles
                var actividadConDetalles = await _actividadRepository.GetActividadDetalladaAsync(actividad.Id);
                var actividadDto = MapActividadConDetallesADto(actividadConDetalles);

                return ApiResponse<ActividadExtracurricularDto>.SuccessResponse(
                    actividadDto,
                    "Actividad creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear actividad extracurricular");

                string userMessage = "Error al crear la actividad";
                List<string> errors = new List<string> { ex.Message };

                if (ex.InnerException != null)
                {
                    errors.Add(ex.InnerException.Message);

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

                return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(userMessage, errors);
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
                var validacionHorario = ValidarHorario(dto.HoraInicio, dto.HoraFin);
                if (validacionHorario != null)
                    return validacionHorario;

                // Actualizar imagen si se proporciona una nueva
                if (imagen != null)
                {
                    if (!string.IsNullOrEmpty(actividad.RutaImagen))
                    {
                        EliminarImagen(actividad.RutaImagen);
                    }
                    actividad.RutaImagen = await GuardarImagenAsync(imagen);
                }

                // Actualizar campos
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

                // Obtener actividad con detalles
                var actividadConDetalles = await _actividadRepository.GetActividadDetalladaAsync(id);
                var actividadDto = MapActividadConDetallesADto(actividadConDetalles);

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
        #endregion

        #region Consultas
        public async Task<ApiResponse<ActividadDetalleDto>> GetByIdAsync(int id, int? estudianteId = null)
        {
            try
            {
                var actividadConDetalles = await _actividadRepository.GetActividadDetalladaAsync(id);
                if (actividadConDetalles == null)
                {
                    return ApiResponse<ActividadDetalleDto>.ErrorResponse("Actividad no encontrada");
                }

                // Obtener estudiantes inscritos
                var estudiantesInscritos = await _inscripcionRepository
                    .GetInscripcionesPorActividadConDetallesAsync(id);

                // Verificar si el estudiante está inscrito
                bool estaInscrito = false;
                if (estudianteId.HasValue)
                {
                    estaInscrito = await _inscripcionRepository.EstaInscritoAsync(estudianteId.Value, id);
                }

                var detalleDto = new ActividadDetalleDto
                {
                    Id = actividadConDetalles.Id,
                    Nombre = actividadConDetalles.Nombre,
                    Descripcion = actividadConDetalles.Descripcion,
                    Categoria = actividadConDetalles.Categoria,
                    Requisitos = actividadConDetalles.Requisitos,
                    HoraInicio = actividadConDetalles.HoraInicio,
                    HoraFin = actividadConDetalles.HoraFin,
                    DiaSemana = ConvertirDayOfWeekADia(actividadConDetalles.DiaSemana),
                    Ubicacion = actividadConDetalles.Ubicacion,
                    ColorTarjeta = actividadConDetalles.ColorTarjeta,
                    RutaImagen = actividadConDetalles.RutaImagen,
                    EstaActiva = actividadConDetalles.EstaActiva,
                    NombreProfesor = actividadConDetalles.ProfesorNombreCompleto,
                    TotalInscritos = actividadConDetalles.TotalInscritos,
                    EstudiantesInscritos = estudiantesInscritos.Select(e => new EstudianteInscritoDto
                    {
                        IdEstudiante = e.IdEstudiante,
                        NombreCompleto = e.EstudianteNombreCompleto,
                        Matricula = e.EstudianteMatricula,
                        FechaInscripcion = e.FechaInscripcion
                    }).ToList(),
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
                var actividadesConDetalles = await _actividadRepository.GetAllActividadesDetalladasAsync();
                var actividadesDto = new List<ActividadExtracurricularDto>();

                foreach (var actividad in actividadesConDetalles)
                {
                    var dto = await MapActividadConDetallesADtoConInscripcion(actividad, estudianteId);
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
                var actividadesConDetalles = await _actividadRepository.GetActividadesActivasDetalladasAsync();
                var actividadesDto = new List<ActividadExtracurricularDto>();

                foreach (var actividad in actividadesConDetalles)
                {
                    var dto = await MapActividadConDetallesADtoConInscripcion(actividad, estudianteId);
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
                var actividadesConDetalles = await _actividadRepository
                    .GetActividadesPorCategoriaDetalladasAsync(categoria);
                var actividadesDto = new List<ActividadExtracurricularDto>();

                foreach (var actividad in actividadesConDetalles)
                {
                    var dto = await MapActividadConDetallesADtoConInscripcion(actividad, estudianteId);
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
                var inscripciones = await _inscripcionRepository
                    .GetInscripcionesPorActividadConDetallesAsync(idActividad);

                var estudiantesDto = inscripciones.Select(i => new EstudianteInscritoDto
                {
                    IdEstudiante = i.IdEstudiante,
                    NombreCompleto = i.EstudianteNombreCompleto,
                    Matricula = i.EstudianteMatricula,
                    FechaInscripcion = i.FechaInscripcion
                }).ToList();

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
        #endregion

        #region Inscripciones
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
        #endregion

        #region Helpers - Validación
        private ApiResponse<ActividadExtracurricularDto> ValidarHorario(TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (horaInicio < TimeSpan.FromHours(14) || horaFin > TimeSpan.FromHours(20))
            {
                return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                    "El horario debe estar entre las 2:00 PM y las 8:00 PM");
            }

            if (horaFin <= horaInicio)
            {
                return ApiResponse<ActividadExtracurricularDto>.ErrorResponse(
                    "La hora de fin debe ser posterior a la hora de inicio");
            }

            return null;
        }
        #endregion

        #region Helpers - Mapeo
        private ActividadExtracurricularDto MapActividadConDetallesADto(Domain.ReadModels.ActividadConDetalles actividad)
        {
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
                NombreProfesor = actividad.ProfesorNombreCompleto,
                TotalInscritos = actividad.TotalInscritos,
                EstaInscrito = false
            };
        }

        private async Task<ActividadExtracurricularDto> MapActividadConDetallesADtoConInscripcion(
            Domain.ReadModels.ActividadConDetalles actividad,
            int? estudianteId)
        {
            var dto = MapActividadConDetallesADto(actividad);

            if (estudianteId.HasValue)
            {
                dto.EstaInscrito = await _inscripcionRepository.EstaInscritoAsync(estudianteId.Value, actividad.Id);
            }

            return dto;
        }
        #endregion

        #region Helpers - Conversión
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
        #endregion

        #region Helpers - Archivos
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
        #endregion
    }
}
