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

                var periodoActivo = await _periodoRepository.GetPeriodoActivoAsync();
                if (periodoActivo == null)
                {
                    return ApiResponse<List<AsignaturaProfesorDto>>.ErrorResponse("No hay período activo");
                }

                var clases = await _claseProgramadaRepository.GetByProfesorAsync(profesor.Id);
                var asignaturas = new List<AsignaturaProfesorDto>();

                foreach (var clase in clases)
                {
                    var inscripciones = await _inscripcionRepository
                        .GetAllByConditionAsync(i => i.IdCursoAcademico == clase.IdCursoAcademico);

                    // ✅ CORREGIR: Obtener calificaciones del curso
                    var calificacionesCurso = await _calificacionRepository.GetCalificacionesPorCursoYAsignaturaAsync(
                        clase.IdCursoAcademico, clase.IdAsignatura, periodoActivo.Id);

                    // ✅ Contar publicadas
                    var calificacionesPublicadas = calificacionesCurso.Count(c => c.Publicada);

                    // ✅ Contar guardadas (total != 0, sin importar si está publicada)
                    var calificacionesGuardadas = calificacionesCurso.Count(c => c.Total > 0);

                    // ✅ Contar pendientes (total == 0 O no existe)
                    var calificacionesPendientes = inscripciones.Count - calificacionesGuardadas;

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
                        CalificacionesPendientes = calificacionesPendientes,
                        CalificacionesGuardadas = calificacionesGuardadas
                    });
                }

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

                // ✅ AGREGAR: Variable para saber si todas están publicadas
                bool todasPublicadas = true;

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

                    if (calificacionExistente != null)
                    {
                        _logger.LogInformation($"📊 Calificación existente para estudiante {dto.Matricula} - Publicada: {calificacionExistente.Publicada}");

                        // ✅ Si al menos una no está publicada
                        if (!calificacionExistente.Publicada)
                        {
                            todasPublicadas = false;
                        }

                        foreach (var detalle in calificacionExistente.Detalles)
                        {
                            dto.Componentes[detalle.IdComponenteCalificacion] = detalle.Valor;
                        }
                        dto.Observaciones = calificacionExistente.Observaciones;
                    }
                    else
                    {
                        todasPublicadas = false;
                    }

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
                    IdProfesor = profesor.Id,
                    TipoAsignatura = asignatura.TipoAsignatura,
                    Componentes = componentes.Select(c => new ComponenteDto
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        ValorMaximo = c.ValorMaximo,
                        Orden = c.Orden
                    }).ToList(),
                    Calificaciones = calificaciones,
                    // ✅ AGREGAR: Indicador de si están publicadas
                    TodasPublicadas = todasPublicadas
                };

                _logger.LogInformation($"✅ Datos preparados correctamente. IdProfesor: {resultado.IdProfesor}, Publicadas: {todasPublicadas}");
                return ApiResponse<CapturaMasivaDto>.SuccessResponse(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener estudiantes para calificar");
                return ApiResponse<CapturaMasivaDto>.ErrorResponse(
                    "Error al obtener estudiantes", new List<string> { ex.Message });
            }
        }

        // ✅ ACTUALIZAR el método de guardar para prevenir modificación de publicadas
        public async Task<ApiResponse<bool>> GuardarCalificacionesAsync(GuardarCalificacionesRequestDto dto)
        {
            try
            {
                _logger.LogInformation($"💾 Guardando calificaciones - IdProfesor: {dto.IdProfesor}, IdAsignatura: {dto.IdAsignatura}");

                // ✅ VALIDAR: Verificar si ya están publicadas
                var calificacionesCurso = await _calificacionRepository.GetCalificacionesPorCursoYAsignaturaAsync(
                    dto.IdCursoAcademico, dto.IdAsignatura, dto.IdPeriodo);

                if (calificacionesCurso.Any(c => c.Publicada))
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "No se pueden modificar calificaciones ya publicadas. Contacte al administrador si necesita hacer cambios.");
                }

                var componentes = await _componenteRepository.GetByTipoAsignaturaAsync(dto.TipoAsignatura);

                foreach (var calificacionDto in dto.Calificaciones)
                {
                    decimal total = 0;
                    foreach (var componenteValor in calificacionDto.Componentes)
                    {
                        var componente = componentes.FirstOrDefault(c => c.Id == componenteValor.Key);
                        if (componente == null) continue;

                        var valor = componenteValor.Value ?? 0;

                        if (valor > componente.ValorMaximo)
                        {
                            return ApiResponse<bool>.ErrorResponse(
                                $"El valor de '{componente.Nombre}' ({valor}) excede el máximo ({componente.ValorMaximo}) para el estudiante ID: {calificacionDto.IdEstudiante}");
                        }

                        total += valor;
                    }

                    if (total > 100)
                    {
                        return ApiResponse<bool>.ErrorResponse(
                            $"El total ({total} pts) excede 100 puntos para el estudiante ID: {calificacionDto.IdEstudiante}");
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
                    _logger.LogWarning($"❌ Estudiante no encontrado para ApplicationUserId: {applicationUserId}");
                    return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse("Estudiante no encontrado");
                }

                _logger.LogInformation($"✅ Estudiante encontrado - ID: {estudiante.Id}");

                // ✅ SOLO obtener calificaciones PUBLICADAS
                var calificaciones = await _calificacionRepository.GetCalificacionesPorEstudianteAsync(estudiante.Id);

                _logger.LogInformation($"📊 Total de calificaciones publicadas encontradas: {calificaciones.Count}");

                if (!calificaciones.Any())
                {
                    _logger.LogInformation("ℹ️ El estudiante no tiene calificaciones publicadas");
                    return ApiResponse<List<CalificacionEstudianteViewDto>>.SuccessResponse(
                        new List<CalificacionEstudianteViewDto>(),
                        "No hay calificaciones publicadas disponibles");
                }

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

                _logger.LogInformation($"✅ Devolviendo {resultado.Count} asignaturas con calificaciones");
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
