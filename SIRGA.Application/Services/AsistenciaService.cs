

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Asistencia;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;

namespace SIRGA.Application.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _asistenciaRepository;
        private readonly IClaseProgramadaRepository _claseProgramadaRepository;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly IProfesorRepository _profesorRepository;
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AsistenciaService> _logger;

        public AsistenciaService(
            IAsistenciaRepository asistenciaRepository,
            IClaseProgramadaRepository claseProgramadaRepository,
            IEstudianteRepository estudianteRepository,
            IProfesorRepository profesorRepository,
            IInscripcionRepository inscripcionRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<AsistenciaService> logger)
        {
            _asistenciaRepository = asistenciaRepository;
            _claseProgramadaRepository = claseProgramadaRepository;
            _estudianteRepository = estudianteRepository;
            _profesorRepository = profesorRepository;
            _inscripcionRepository = inscripcionRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<AsistenciaResponseDto>> RegistrarAsistenciaAsync(
            RegistrarAsistenciaDto dto,
            string registradoPorId)
        {
            try
            {
                // Validar que no exista ya una asistencia para ese estudiante en esa clase y fecha
                var existente = await _asistenciaRepository.ExisteAsistenciaAsync(
                    dto.IdEstudiante,
                    dto.IdClaseProgramada,
                    dto.Fecha);

                if (existente)
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                        "Ya existe un registro de asistencia para este estudiante en esta clase y fecha");
                }

                // Validar que el estudiante esté inscrito en el curso académico de esta clase
                var clase = await _claseProgramadaRepository.GetByIdAsync(dto.IdClaseProgramada);
                if (clase == null)
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse("Clase no encontrada");
                }

                var estudianteInscrito = await _inscripcionRepository.GetAllByConditionAsync(
                    i => i.IdEstudiante == dto.IdEstudiante &&
                         i.IdCursoAcademico == clase.IdCursoAcademico);

                if (!estudianteInscrito.Any())
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                        "El estudiante no está inscrito en este curso académico");
                }

                // Determinar si requiere justificación automáticamente
                bool requiereJustificacion = dto.RequiereJustificacion ||
                                            dto.Estado == "Ausente" ||
                                            dto.Estado == "Tarde";

                var asistencia = new Asistencia
                {
                    IdEstudiante = dto.IdEstudiante,
                    IdClaseProgramada = dto.IdClaseProgramada,
                    IdProfesor = clase.IdProfesor,
                    Fecha = dto.Fecha.Date,
                    HoraRegistro = DateTime.Now,
                    Estado = dto.Estado,
                    Observaciones = dto.Observaciones,
                    RequiereJustificacion = requiereJustificacion,
                    RegistradoPorId = registradoPorId,

                    // ✅ NUEVO: Si viene justificación, guardarla inmediatamente
                    Justificacion = !string.IsNullOrWhiteSpace(dto.Justificacion) ? dto.Justificacion : null,
                    FechaJustificacion = !string.IsNullOrWhiteSpace(dto.Justificacion) ? DateTime.Now : null,
                    UsuarioJustificacionId = !string.IsNullOrWhiteSpace(dto.Justificacion) ? registradoPorId : null
                };

                // Si tiene justificación, cambiar el estado a "Justificado"
                if (!string.IsNullOrWhiteSpace(dto.Justificacion))
                {
                    asistencia.Estado = "Justificado";
                }

                var resultado = await _asistenciaRepository.AddAsync(asistencia);

                return ApiResponse<AsistenciaResponseDto>.SuccessResponse(
                    await MapToResponseDto(resultado),
                    "Asistencia registrada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar asistencia");
                return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                    "Error al registrar la asistencia",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AsistenciaResponseDto>>> RegistrarAsistenciaMasivaAsync(
            RegistrarAsistenciaMasivaDto dto,
            string registradoPorId)
        {
            try
            {
                var clase = await _claseProgramadaRepository.GetByIdAsync(dto.IdClaseProgramada);
                if (clase == null)
                {
                    return ApiResponse<List<AsistenciaResponseDto>>.ErrorResponse("Clase no encontrada");
                }

                var asistenciasRegistradas = new List<Asistencia>();
                var errores = new List<string>();

                foreach (var asistenciaDto in dto.Asistencias)
                {
                    try
                    {
                        // Verificar si ya existe
                        var existente = await _asistenciaRepository.ExisteAsistenciaAsync(
                            asistenciaDto.IdEstudiante,
                            dto.IdClaseProgramada,
                            dto.Fecha);

                        if (existente)
                        {
                            errores.Add($"Estudiante {asistenciaDto.IdEstudiante} ya tiene asistencia registrada");
                            continue;
                        }

                        bool requiereJustificacion = asistenciaDto.RequiereJustificacion ||
                                                    asistenciaDto.Estado == "Ausente" ||
                                                    asistenciaDto.Estado == "Tarde";

                        var asistencia = new Asistencia
                        {
                            IdEstudiante = asistenciaDto.IdEstudiante,
                            IdClaseProgramada = dto.IdClaseProgramada,
                            IdProfesor = clase.IdProfesor,
                            Fecha = dto.Fecha.Date,
                            HoraRegistro = DateTime.Now,
                            Estado = asistenciaDto.Estado,
                            Observaciones = asistenciaDto.Observaciones,
                            RequiereJustificacion = requiereJustificacion,
                            RegistradoPorId = registradoPorId,

                            // ✅ AHORA SÍ FUNCIONA: asistenciaDto tiene el campo Justificacion
                            Justificacion = !string.IsNullOrWhiteSpace(asistenciaDto.Justificacion)
                                ? asistenciaDto.Justificacion
                                : null,
                            FechaJustificacion = !string.IsNullOrWhiteSpace(asistenciaDto.Justificacion)
                                ? DateTime.Now
                                : null,
                            UsuarioJustificacionId = !string.IsNullOrWhiteSpace(asistenciaDto.Justificacion)
                                ? registradoPorId
                                : null
                        };

                        // Si tiene justificación, cambiar el estado a "Justificado"
                        if (!string.IsNullOrWhiteSpace(asistenciaDto.Justificacion))
                        {
                            asistencia.Estado = "Justificado";
                        }

                        _logger.LogInformation($"🔍 Guardando asistencia - Estudiante: {asistenciaDto.IdEstudiante}, Estado: {asistencia.Estado}, Justificacion: {asistencia.Justificacion ?? "NULL"}");

                        var resultado = await _asistenciaRepository.AddAsync(asistencia);
                        asistenciasRegistradas.Add(resultado);
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Error con estudiante {asistenciaDto.IdEstudiante}: {ex.Message}");
                    }
                }

                var responses = new List<AsistenciaResponseDto>();
                foreach (var asistencia in asistenciasRegistradas)
                {
                    responses.Add(await MapToResponseDto(asistencia));
                }

                var mensaje = errores.Any()
                    ? $"Asistencia registrada con {errores.Count} errores"
                    : "Asistencia masiva registrada exitosamente";

                return errores.Any()
                    ? ApiResponse<List<AsistenciaResponseDto>>.ErrorResponse(mensaje, errores)
                    : ApiResponse<List<AsistenciaResponseDto>>.SuccessResponse(responses, mensaje);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar asistencia masiva");
                return ApiResponse<List<AsistenciaResponseDto>>.ErrorResponse(
                    "Error al registrar la asistencia masiva",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AsistenciaResponseDto>> ActualizarAsistenciaAsync(
           int id,
           ActualizarAsistenciaDto dto,
           string modificadoPorId)
        {
            try
            {
                var asistencia = await _asistenciaRepository.GetByIdAsync(id);
                if (asistencia == null)
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse("Asistencia no encontrada");
                }

                // No permitir cambiar a "Presente" si ya tiene justificación
                if (dto.Estado == "Presente" && !string.IsNullOrEmpty(asistencia.Justificacion))
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                        "No se puede cambiar a 'Presente' una asistencia que ya tiene justificación");
                }

                asistencia.Estado = dto.Estado;
                asistencia.Observaciones = dto.Observaciones;
                asistencia.RequiereJustificacion = dto.RequiereJustificacion ||
                                                   dto.Estado == "Ausente" ||
                                                   dto.Estado == "Tarde";
                asistencia.UltimaModificacion = DateTime.Now;
                asistencia.ModificadoPorId = modificadoPorId;

                await _asistenciaRepository.UpdateAsync(asistencia);

                return ApiResponse<AsistenciaResponseDto>.SuccessResponse(
                    await MapToResponseDto(asistencia),
                    "Asistencia actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia");
                return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                    "Error al actualizar la asistencia",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AsistenciaResponseDto>> JustificarAsistenciaAsync(
            int id,
            JustificarAsistenciaDto dto,
            string usuarioJustificacionId)
        {
            try
            {
                var asistencia = await _asistenciaRepository.GetByIdAsync(id);
                if (asistencia == null)
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse("Asistencia no encontrada");
                }

                // Solo se pueden justificar "Tarde" o "Ausente", no "Presente"
                if (asistencia.Estado == "Presente")
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                        "No se puede justificar una asistencia marcada como 'Presente'");
                }

                asistencia.Justificacion = dto.Justificacion;
                asistencia.FechaJustificacion = DateTime.Now;
                asistencia.UsuarioJustificacionId = usuarioJustificacionId;
                asistencia.Estado = "Justificado";
                asistencia.UltimaModificacion = DateTime.Now;
                asistencia.ModificadoPorId = usuarioJustificacionId;

                await _asistenciaRepository.UpdateAsync(asistencia);

                return ApiResponse<AsistenciaResponseDto>.SuccessResponse(
                    await MapToResponseDto(asistencia),
                    "Asistencia justificada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al justificar asistencia");
                return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                    "Error al justificar la asistencia",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<ClaseDelDiaDto>>> GetClasesDelDiaAsync(int idProfesor, DateTime fecha)
        {
            try
            {
                var diaSemana = fecha.DayOfWeek;

                var clases = await _claseProgramadaRepository.GetClasesByProfesorAndDayAsync(idProfesor, diaSemana);

                var clasesDelDia = new List<ClaseDelDiaDto>();

                foreach (var clase in clases)
                {
                    // Obtener estudiantes inscritos en este curso
                    var inscripcionesList = await _inscripcionRepository.GetAllByConditionAsync(
                        i => i.IdCursoAcademico == clase.IdCursoAcademico);

                    // Obtener asistencias ya registradas para esta clase en esta fecha
                    var asistenciasList = await _asistenciaRepository.GetAsistenciasByClaseAndFechaAsync(
                        clase.Id, fecha);

                    clasesDelDia.Add(new ClaseDelDiaDto
                    {
                        IdClaseProgramada = clase.Id,
                        AsignaturaNombre = clase.Asignatura.Nombre,
                        HoraInicio = clase.StartTime,
                        HoraFin = clase.EndTime,
                        DiaSemana = clase.WeekDay,
                        Location = clase.Location,
                        CantidadEstudiantes = inscripcionesList.Count,
                        AsistenciaRegistrada = asistenciasList.Any(),
                        EstudiantesPresentes = asistenciasList.Count(a => a.Estado == "Presente"),
                        EstudiantesAusentes = asistenciasList.Count(a => a.Estado == "Ausente"),
                        EstudiantesTarde = asistenciasList.Count(a => a.Estado == "Tarde"),
                        EstudiantesJustificados = asistenciasList.Count(a => a.Estado == "Justificado"),
                        GradoNombre = clase.CursoAcademico.Grado.GradeName,
                        GradoSeccion = clase.CursoAcademico.Grado.Section
                    });
                }

                return ApiResponse<List<ClaseDelDiaDto>>.SuccessResponse(
                    clasesDelDia.OrderBy(c => c.HoraInicio).ToList(),
                    "Clases del día obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clases del día");
                return ApiResponse<List<ClaseDelDiaDto>>.ErrorResponse(
                    "Error al obtener las clases del día",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<EstudianteClaseDto>>> GetEstudiantesPorClaseAsync(
            int idClaseProgramada,
            DateTime fecha)
        {
            try
            {
                var clase = await _claseProgramadaRepository.GetByIdAsync(idClaseProgramada);
                if (clase == null)
                {
                    return ApiResponse<List<EstudianteClaseDto>>.ErrorResponse("Clase no encontrada");
                }

                // Obtener estudiantes inscritos en este curso académico
                var inscripcionesList = await _inscripcionRepository.GetAllByConditionAsync(
                    i => i.IdCursoAcademico == clase.IdCursoAcademico);

                var estudiantes = new List<EstudianteClaseDto>();

                foreach (var inscripcion in inscripcionesList)
                {
                    var estudiante = await _estudianteRepository.GetByIdAsync(inscripcion.IdEstudiante);
                    var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);

                    // Verificar si ya tiene asistencia registrada
                    var asistenciaExistente = await _asistenciaRepository.GetAsistenciaByEstudianteClaseFechaAsync(
                        estudiante.Id, idClaseProgramada, fecha);

                    estudiantes.Add(new EstudianteClaseDto
                    {
                        IdEstudiante = estudiante.Id,
                        Matricula = estudiante.Matricula,
                        Nombre = user.FirstName,
                        Apellido = user.LastName,
                        Email = user.Email,
                        Photo = user.Photo,
                        AsistenciaId = asistenciaExistente?.Id,
                        EstadoAsistencia = asistenciaExistente?.Estado,
                        YaRegistrada = asistenciaExistente != null
                    });
                }

                // Ordenar alfabéticamente por apellido, luego por nombre
                var estudiantesOrdenados = estudiantes
                    .OrderBy(e => e.Apellido)
                    .ThenBy(e => e.Nombre)
                    .ToList();

                return ApiResponse<List<EstudianteClaseDto>>.SuccessResponse(
                    estudiantesOrdenados,
                    "Estudiantes obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estudiantes por clase");
                return ApiResponse<List<EstudianteClaseDto>>.ErrorResponse(
                    "Error al obtener los estudiantes",
                    new List<string> { ex.Message });
            }
        }

        // Métodos de mapeo y otros...
        private async Task<AsistenciaResponseDto> MapToResponseDto(Asistencia asistencia)
        {
            var estudiante = await _estudianteRepository.GetByIdAsync(asistencia.IdEstudiante);
            var userEstudiante = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);

            var profesor = await _profesorRepository.GetByIdAsync(asistencia.IdProfesor);
            var userProfesor = await _userManager.FindByIdAsync(profesor.ApplicationUserId);

            var clase = await _claseProgramadaRepository.GetByIdAsync(asistencia.IdClaseProgramada);

            return new AsistenciaResponseDto
            {
                Id = asistencia.Id,
                Fecha = asistencia.Fecha,
                HoraRegistro = asistencia.HoraRegistro,
                Estado = asistencia.Estado,
                Observaciones = asistencia.Observaciones,
                RequiereJustificacion = asistencia.RequiereJustificacion,
                Justificacion = asistencia.Justificacion,
                FechaJustificacion = asistencia.FechaJustificacion,
                IdEstudiante = estudiante.Id,
                EstudianteNombre = userEstudiante.FirstName,
                EstudianteApellido = userEstudiante.LastName,
                EstudianteMatricula = estudiante.Matricula,
                IdClaseProgramada = clase.Id,
                AsignaturaNombre = clase.Asignatura.Nombre,
                ClaseLocation = clase.Location,
                HoraInicio = clase.StartTime,
                HoraFin = clase.EndTime,
                IdProfesor = profesor.Id,
                ProfesorNombre = userProfesor.FirstName,
                ProfesorApellido = userProfesor.LastName,
                RegistradoPorId = asistencia.RegistradoPorId,
                UltimaModificacion = asistencia.UltimaModificacion,
                ModificadoPorId = asistencia.ModificadoPorId,
                UsuarioJustificacionId = asistencia.UsuarioJustificacionId
            };
        }
        public async Task<ApiResponse<List<AsistenciaResponseDto>>> GetHistorialAsistenciaEstudianteAsync(
            int idEstudiante,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                var asistencias = await _asistenciaRepository.GetAsistenciasByEstudianteAsync(
                    idEstudiante, fechaInicio, fechaFin);

                var responses = new List<AsistenciaResponseDto>();
                foreach (var asistencia in asistencias)
                {
                    responses.Add(await MapToResponseDto(asistencia));
                }

                return ApiResponse<List<AsistenciaResponseDto>>.SuccessResponse(
                    responses,
                    "Historial de asistencia obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de asistencia del estudiante");
                return ApiResponse<List<AsistenciaResponseDto>>.ErrorResponse(
                    "Error al obtener el historial",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AsistenciaResponseDto>>> GetHistorialAsistenciaClaseAsync(
            int idClaseProgramada,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            try
            {
                var asistencias = await _asistenciaRepository.GetHistorialAsistenciasAsync(
                    idClaseProgramada, fechaInicio, fechaFin);

                var responses = new List<AsistenciaResponseDto>();
                foreach (var asistencia in asistencias)
                {
                    responses.Add(await MapToResponseDto(asistencia));
                }

                return ApiResponse<List<AsistenciaResponseDto>>.SuccessResponse(
                    responses,
                    "Historial de asistencia de la clase obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de asistencia de la clase");
                return ApiResponse<List<AsistenciaResponseDto>>.ErrorResponse(
                    "Error al obtener el historial",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AsistenciaResponseDto>>> GetAsistenciasRequierenJustificacionAsync()
        {
            try
            {
                var asistencias = await _asistenciaRepository.GetAsistenciasRequierenJustificacionAsync();

                var responses = new List<AsistenciaResponseDto>();
                foreach (var asistencia in asistencias)
                {
                    responses.Add(await MapToResponseDto(asistencia));
                }

                return ApiResponse<List<AsistenciaResponseDto>>.SuccessResponse(
                    responses,
                    "Asistencias que requieren justificación obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias que requieren justificación");
                return ApiResponse<List<AsistenciaResponseDto>>.ErrorResponse(
                    "Error al obtener las asistencias",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<EstadisticasAsistenciaDto>> GetEstadisticasEstudianteAsync(
            int idEstudiante,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                var asistencias = await _asistenciaRepository.GetAsistenciasByEstudianteAsync(
                    idEstudiante, fechaInicio, fechaFin);

                var estadisticas = new EstadisticasAsistenciaDto
                {
                    TotalClases = asistencias.Count,
                    TotalPresentes = asistencias.Count(a => a.Estado == "Presente"),
                    TotalAusentes = asistencias.Count(a => a.Estado == "Ausente"),
                    TotalTardes = asistencias.Count(a => a.Estado == "Tarde"),
                    TotalJustificados = asistencias.Count(a => a.Estado == "Justificado")
                };

                if (estadisticas.TotalClases > 0)
                {
                    estadisticas.PorcentajeAsistencia =
                        ((decimal)(estadisticas.TotalPresentes + estadisticas.TotalJustificados) /
                         estadisticas.TotalClases) * 100;
                }

                return ApiResponse<EstadisticasAsistenciaDto>.SuccessResponse(
                    estadisticas,
                    "Estadísticas obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return ApiResponse<EstadisticasAsistenciaDto>.ErrorResponse(
                    "Error al obtener las estadísticas",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AsistenciaResponseDto>> GetAsistenciaByIdAsync(int id)
        {
            try
            {
                var asistencia = await _asistenciaRepository.GetByIdAsync(id);
                if (asistencia == null)
                {
                    return ApiResponse<AsistenciaResponseDto>.ErrorResponse("Asistencia no encontrada");
                }

                return ApiResponse<AsistenciaResponseDto>.SuccessResponse(
                    await MapToResponseDto(asistencia),
                    "Asistencia obtenida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia");
                return ApiResponse<AsistenciaResponseDto>.ErrorResponse(
                    "Error al obtener la asistencia",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> EliminarAsistenciaAsync(int id)
        {
            try
            {
                var asistencia = await _asistenciaRepository.GetByIdAsync(id);
                if (asistencia == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Asistencia no encontrada");
                }

                await _asistenciaRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Asistencia eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar asistencia");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la asistencia",
                    new List<string> { ex.Message });
            }
        }
    }
}
