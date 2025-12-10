using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Calificacion;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.Repositories.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIRGA.Application.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly ICalificacionRepository _calificacionRepository;
        private readonly IProfesorRepository _profesorRepository;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly IComponenteCalificacionRepository _componenteRepository;
        private readonly ICalificacionDetalleRepository _detalleRepository;
        private readonly IHistorialCalificacionRepository _historialRepository;
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IClaseProgramadaRepository _claseProgramadaRepository;
        private readonly IPeriodoRepository _periodoRepository;
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CalificacionService> _logger;

        public CalificacionService(
            ICalificacionRepository calificacionRepository,
            IProfesorRepository profesorRepository,
            IEstudianteRepository estudianteRepository,
            IComponenteCalificacionRepository componenteRepository,
            ICalificacionDetalleRepository detalleRepository,
            IHistorialCalificacionRepository historialRepository,
            IInscripcionRepository inscripcionRepository,
            IClaseProgramadaRepository claseProgramadaRepository,
            IPeriodoRepository periodoRepository,
            IAsignaturaRepository asignaturaRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<CalificacionService> logger)
        {
            _calificacionRepository = calificacionRepository;
            _profesorRepository = profesorRepository;
            _estudianteRepository = estudianteRepository;
            _componenteRepository = componenteRepository;
            _detalleRepository = detalleRepository;
            _historialRepository = historialRepository;
            _inscripcionRepository = inscripcionRepository;
            _claseProgramadaRepository = claseProgramadaRepository;
            _periodoRepository = periodoRepository;
            _asignaturaRepository = asignaturaRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<List<AsignaturaProfesorDto>>> GetAsignaturasProfesorAsync(string applicationUserId)
        {
            try
            {
                _logger.LogInformation($"🔍 GetAsignaturasProfesorAsync - ApplicationUserId: {applicationUserId}");

                var profesor = await _profesorRepository.GetByApplicationUserIdAsync(applicationUserId);
                if (profesor == null)
                {
                    _logger.LogWarning($"❌ Profesor no encontrado para ApplicationUserId: {applicationUserId}");
                    return ApiResponse<List<AsignaturaProfesorDto>>.ErrorResponse("Profesor no encontrado");
                }

                _logger.LogInformation($"✅ Profesor encontrado - ID: {profesor.Id}");

                var periodoActivo = await _periodoRepository.GetPeriodoActivoAsync();
                if (periodoActivo == null)
                {
                    return ApiResponse<List<AsignaturaProfesorDto>>.ErrorResponse("No hay período activo");
                }

                _logger.LogInformation($"✅ Período activo: {periodoActivo.Id} - Número: {periodoActivo.Numero}");

                var clases = await _claseProgramadaRepository.GetByProfesorAsync(profesor.Id);
                _logger.LogInformation($"📚 Total de clases del profesor: {clases.Count}");

                var asignaturas = new List<AsignaturaProfesorDto>();

                foreach (var clase in clases)
                {
                    var inscripciones = await _inscripcionRepository
                        .GetAllByConditionAsync(i => i.IdCursoAcademico == clase.IdCursoAcademico);

                    var calificacionesPublicadas = await _calificacionRepository
                        .ContarCalificacionesPublicadasAsync(profesor.Id, periodoActivo.Id);

                    var calificacionesPendientes = await _calificacionRepository
                        .ContarCalificacionesPendientesAsync(profesor.Id, periodoActivo.Id);

                    asignaturas.Add(new AsignaturaProfesorDto
                    {
                        IdAsignatura = clase.IdAsignatura,
                        AsignaturaNombre = clase.Asignatura.Nombre,
                        TipoAsignatura = clase.Asignatura.TipoAsignatura,
                        IdCursoAcademico = clase.IdCursoAcademico,
                        CursoNombre = $"{clase.CursoAcademico.Grado.GradeName} {clase.CursoAcademico.Grado.Nivel}",
                        GradoNombre = clase.CursoAcademico.Grado.GradeName,
                        SeccionNombre = clase.CursoAcademico.Seccion.Nombre,
                        CantidadEstudiantes = inscripciones.Count,
                        CalificacionesPublicadas = calificacionesPublicadas,
                        CalificacionesPendientes = calificacionesPendientes
                    });
                }

                _logger.LogInformation($"✅ Total de asignaturas devueltas: {asignaturas.Count}");
                return ApiResponse<List<AsignaturaProfesorDto>>.SuccessResponse(asignaturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener asignaturas del profesor");
                return ApiResponse<List<AsignaturaProfesorDto>>.ErrorResponse(
                    "Error al obtener asignaturas", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CapturaMasivaDto>> GetEstudiantesParaCalificarAsync(
            string applicationUserId, int idAsignatura, int idCursoAcademico)
        {
            try
            {
                _logger.LogInformation($"🔍 GetEstudiantesParaCalificarAsync - UserId: {applicationUserId}, Asignatura: {idAsignatura}, Curso: {idCursoAcademico}");

                // PASO 1: Obtener el profesor por ApplicationUserId
                var profesor = await _profesorRepository.GetByApplicationUserIdAsync(applicationUserId);
                if (profesor == null)
                {
                    _logger.LogWarning($"❌ Profesor no encontrado para ApplicationUserId: {applicationUserId}");
                    return ApiResponse<CapturaMasivaDto>.ErrorResponse("Profesor no encontrado");
                }

                _logger.LogInformation($"✅ Profesor encontrado - ID numérico: {profesor.Id}");

                var periodoActivo = await _periodoRepository.GetPeriodoActivoAsync();
                if (periodoActivo == null)
                {
                    return ApiResponse<CapturaMasivaDto>.ErrorResponse("No hay período activo");
                }

                var asignatura = await _asignaturaRepository.GetByIdAsync(idAsignatura);
                if (asignatura == null)
                {
                    return ApiResponse<CapturaMasivaDto>.ErrorResponse("Asignatura no encontrada");
                }

                _logger.LogInformation($"📚 Asignatura: {asignatura.Nombre}, Tipo: {asignatura.TipoAsignatura}");

                var componentes = await _componenteRepository.GetByTipoAsignaturaAsync(asignatura.TipoAsignatura);
                _logger.LogInformation($"📝 Componentes encontrados: {componentes.Count}");

                var inscripciones = await _inscripcionRepository
                    .GetAllByConditionAsync(i => i.IdCursoAcademico == idCursoAcademico);

                _logger.LogInformation($"👥 Estudiantes inscritos: {inscripciones.Count}");

                var calificaciones = new List<CalificacionEstudianteDto>();

                foreach (var inscripcion in inscripciones)
                {
                    var calificacionExistente = await _calificacionRepository.GetCalificacionConDetallesAsync(
                        inscripcion.IdEstudiante, idAsignatura, periodoActivo.Id);

                    var user = await _userManager.FindByIdAsync(inscripcion.Estudiante.ApplicationUserId);

                    var dto = new CalificacionEstudianteDto
                    {
                        IdEstudiante = inscripcion.IdEstudiante,
                        Matricula = inscripcion.Estudiante.Matricula,
                        NombreCompleto = user != null ? $"{user.FirstName} {user.LastName}" : "N/A",
                        Componentes = new Dictionary<int, decimal?>()
                    };

                    // Si ya existe calificación, cargar los valores
                    if (calificacionExistente != null)
                    {
                        _logger.LogInformation($"📊 Calificación existente para estudiante {dto.Matricula}");
                        foreach (var detalle in calificacionExistente.Detalles)
                        {
                            dto.Componentes[detalle.IdComponenteCalificacion] = detalle.Valor;
                        }
                        dto.Observaciones = calificacionExistente.Observaciones;
                    }

                    // Asegurar que todos los componentes están presentes
                    foreach (var componente in componentes)
                    {
                        if (!dto.Componentes.ContainsKey(componente.Id))
                        {
                            dto.Componentes[componente.Id] = null;
                        }
                    }

                    calificaciones.Add(dto);
                }

                var resultado = new CapturaMasivaDto
                {
                    IdAsignatura = idAsignatura,
                    IdCursoAcademico = idCursoAcademico,
                    IdPeriodo = periodoActivo.Id,
                    IdProfesor = profesor.Id, // ✅ Ahora usamos el ID numérico correcto
                    TipoAsignatura = asignatura.TipoAsignatura,
                    Componentes = componentes.Select(c => new ComponenteDto
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        ValorMaximo = c.ValorMaximo,
                        Orden = c.Orden
                    }).ToList(),
                    Calificaciones = calificaciones
                };

                _logger.LogInformation($"✅ Datos preparados correctamente. IdProfesor: {resultado.IdProfesor}");
                return ApiResponse<CapturaMasivaDto>.SuccessResponse(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener estudiantes para calificar");
                return ApiResponse<CapturaMasivaDto>.ErrorResponse(
                    "Error al obtener estudiantes", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> GuardarCalificacionesAsync(CapturaMasivaDto dto)
        {
            try
            {
                _logger.LogInformation($"💾 Guardando calificaciones - IdProfesor: {dto.IdProfesor}, IdAsignatura: {dto.IdAsignatura}");

                var componentes = await _componenteRepository.GetByTipoAsignaturaAsync(dto.TipoAsignatura);

                foreach (var calificacionDto in dto.Calificaciones)
                {
                    // Calcular total primero para validación
                    decimal total = 0;
                    foreach (var componenteValor in calificacionDto.Componentes)
                    {
                        var componente = componentes.FirstOrDefault(c => c.Id == componenteValor.Key);
                        if (componente == null) continue;

                        var valor = componenteValor.Value ?? 0;

                        // Validar que no exceda el máximo del componente
                        if (valor > componente.ValorMaximo)
                        {
                            return ApiResponse<bool>.ErrorResponse(
                                $"El valor de '{componente.Nombre}' ({valor}) excede el máximo ({componente.ValorMaximo}) para el estudiante {calificacionDto.Matricula}");
                        }

                        total += valor;
                    }

                    // Validar que el total no exceda 100
                    if (total > 100)
                    {
                        return ApiResponse<bool>.ErrorResponse(
                            $"El total ({total} pts) excede 100 puntos para el estudiante {calificacionDto.Matricula}");
                    }

                    var calificacionExistente = await _calificacionRepository.GetCalificacionConDetallesAsync(
                        calificacionDto.IdEstudiante, dto.IdAsignatura, dto.IdPeriodo);

                    Calificacion calificacion;
                    bool esNueva = false;

                    if (calificacionExistente == null)
                    {
                        _logger.LogInformation($"➕ Creando nueva calificación para estudiante {calificacionDto.IdEstudiante}");
                        calificacion = new Calificacion
                        {
                            IdEstudiante = calificacionDto.IdEstudiante,
                            IdAsignatura = dto.IdAsignatura,
                            IdCursoAcademico = dto.IdCursoAcademico,
                            IdPeriodo = dto.IdPeriodo,
                            IdProfesor = dto.IdProfesor,
                            FechaCreacion = DateTime.Now,
                            Detalles = new List<CalificacionDetalle>()
                        };
                        esNueva = true;
                    }
                    else
                    {
                        _logger.LogInformation($"✏️ Actualizando calificación existente ID: {calificacionExistente.Id}");
                        calificacion = calificacionExistente;
                    }

                    // Guardar detalles
                    foreach (var componenteValor in calificacionDto.Componentes)
                    {
                        var componente = componentes.FirstOrDefault(c => c.Id == componenteValor.Key);
                        if (componente == null) continue;

                        var valor = componenteValor.Value ?? 0;

                        var detalleExistente = calificacion.Detalles?
                            .FirstOrDefault(d => d.IdComponenteCalificacion == componenteValor.Key);

                        if (detalleExistente != null)
                        {
                            detalleExistente.Valor = valor;
                        }
                        else
                        {
                            var nuevoDetalle = new CalificacionDetalle
                            {
                                IdComponenteCalificacion = componenteValor.Key,
                                Valor = valor
                            };

                            if (esNueva)
                            {
                                calificacion.Detalles.Add(nuevoDetalle);
                            }
                            else
                            {
                                nuevoDetalle.IdCalificacion = calificacion.Id;
                                await _detalleRepository.AddAsync(nuevoDetalle);
                            }
                        }
                    }

                    calificacion.Total = total;
                    calificacion.Observaciones = calificacionDto.Observaciones;
                    calificacion.FechaUltimaModificacion = DateTime.Now;

                    if (esNueva)
                    {
                        await _calificacionRepository.AddAsync(calificacion);
                        _logger.LogInformation($"✅ Nueva calificación guardada con total: {total}");
                    }
                    else
                    {
                        await _calificacionRepository.UpdateAsync(calificacion);
                        _logger.LogInformation($"✅ Calificación actualizada con total: {total}");
                    }
                }

                return ApiResponse<bool>.SuccessResponse(true, "Calificaciones guardadas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar calificaciones");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al guardar calificaciones", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> PublicarCalificacionesAsync(PublicarCalificacionesDto dto)
        {
            try
            {
                _logger.LogInformation($"📤 Publicando calificaciones - Asignatura: {dto.IdAsignatura}, Curso: {dto.IdCursoAcademico}");

                var calificaciones = await _calificacionRepository.GetCalificacionesPorCursoYAsignaturaAsync(
                    dto.IdCursoAcademico, dto.IdAsignatura, dto.IdPeriodo);

                if (!calificaciones.Any())
                {
                    return ApiResponse<bool>.ErrorResponse("No hay calificaciones para publicar");
                }

                var calificacionesIncompletas = calificaciones.Where(c => c.Total == 0).ToList();
                if (calificacionesIncompletas.Any())
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"Hay {calificacionesIncompletas.Count} estudiante(s) sin calificación registrada");
                }

                foreach (var calificacion in calificaciones)
                {
                    calificacion.Publicada = true;
                    calificacion.FechaPublicacion = DateTime.Now;
                    await _calificacionRepository.UpdateAsync(calificacion);
                }

                _logger.LogInformation($"✅ {calificaciones.Count} calificaciones publicadas exitosamente");
                return ApiResponse<bool>.SuccessResponse(
                    true, $"{calificaciones.Count} calificaciones publicadas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al publicar calificaciones");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al publicar", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<CalificacionEstudianteViewDto>>> GetCalificacionesEstudianteAsync(string applicationUserId)
        {
            try
            {
                _logger.LogInformation($"🔍 GetCalificacionesEstudianteAsync - UserId: {applicationUserId}");

                var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(applicationUserId);
                if (estudiante == null)
                {
                    return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse("Estudiante no encontrado");
                }

                var calificaciones = await _calificacionRepository.GetCalificacionesPorEstudianteAsync(estudiante.Id);

                var resultado = calificaciones
                    .GroupBy(c => new { c.IdAsignatura, c.Asignatura.Nombre, c.Asignatura.TipoAsignatura })
                    .Select(g => new CalificacionEstudianteViewDto
                    {
                        AsignaturaNombre = g.Key.Nombre,
                        TipoAsignatura = g.Key.TipoAsignatura,
                        Periodos = g.OrderBy(c => c.Periodo.Numero).Select(c => new CalificacionPorPeriodoDto
                        {
                            NumeroPeriodo = c.Periodo.Numero,
                            Componentes = c.Detalles.ToDictionary(
                                d => d.Componente.Nombre,
                                d => (decimal?)d.Valor
                            ),
                            Total = c.Total,
                            Publicada = c.Publicada
                        }).ToList(),
                        PromedioGeneral = (decimal)(g.Where(c => c.Publicada).Average(c => (double?)c.Total) ?? 0)
                    })
                    .ToList();

                return ApiResponse<List<CalificacionEstudianteViewDto>>.SuccessResponse(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener calificaciones del estudiante");
                return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse(
                    "Error al obtener calificaciones", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> EditarCalificacionAsync(
            EditarCalificacionDto dto, string usuarioId, string usuarioNombre, string rol)
        {
            try
            {
                var calificacion = await _calificacionRepository.GetByIdAsync(dto.IdCalificacion);
                if (calificacion == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Calificación no encontrada");
                }

                if (!calificacion.Publicada)
                {
                    return ApiResponse<bool>.ErrorResponse("Solo se pueden editar calificaciones publicadas");
                }

                // Guardar valores anteriores
                var detallesAnteriores = await _detalleRepository.GetDetallesPorCalificacionAsync(calificacion.Id);
                var valoresAnteriores = detallesAnteriores.ToDictionary(
                    d => d.Componente.Nombre,
                    d => d.Valor
                );

                var historial = new HistorialCalificacion
                {
                    IdCalificacion = calificacion.Id,
                    UsuarioModificadorId = usuarioId,
                    NombreUsuarioModificador = usuarioNombre,
                    RolUsuarioModificador = rol,
                    MotivoModificacion = dto.MotivoModificacion,
                    ValoresAnteriores = JsonSerializer.Serialize(valoresAnteriores),
                    TotalAnterior = calificacion.Total
                };

                // Actualizar detalles
                decimal nuevoTotal = 0;
                foreach (var componenteValor in dto.NuevaCalificacion.Componentes)
                {
                    var detalle = detallesAnteriores
                        .FirstOrDefault(d => d.IdComponenteCalificacion == componenteValor.Key);

                    if (detalle != null)
                    {
                        detalle.Valor = componenteValor.Value ?? 0;
                        nuevoTotal += detalle.Valor;
                        await _detalleRepository.UpdateAsync(detalle);
                    }
                }

                calificacion.Total = nuevoTotal;
                calificacion.Observaciones = dto.NuevaCalificacion.Observaciones;
                calificacion.FechaUltimaModificacion = DateTime.Now;

                var valoresNuevos = detallesAnteriores.ToDictionary(
                    d => d.Componente.Nombre,
                    d => d.Valor
                );

                historial.ValoresNuevos = JsonSerializer.Serialize(valoresNuevos);
                historial.TotalNuevo = calificacion.Total;

                await _historialRepository.RegistrarCambioAsync(historial);
                await _calificacionRepository.UpdateAsync(calificacion);

                return ApiResponse<bool>.SuccessResponse(true, "Calificación editada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al editar calificación");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al editar calificación", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<HistorialCalificacion>>> GetHistorialCalificacionAsync(int idCalificacion)
        {
            try
            {
                var historial = await _historialRepository.GetHistorialPorCalificacionAsync(idCalificacion);
                return ApiResponse<List<HistorialCalificacion>>.SuccessResponse(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener historial");
                return ApiResponse<List<HistorialCalificacion>>.ErrorResponse(
                    "Error al obtener historial", new List<string> { ex.Message });
            }
        }
    }
}
