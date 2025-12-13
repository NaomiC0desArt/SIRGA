using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Calificacion;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Interfaces.IA;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.Repositories;
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
        private readonly IHistorialCalificacionRepository _historialCalificacionRepository;
        private readonly IIACalificacionService _iaService;

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
            IIACalificacionService iaService,
            IHistorialCalificacionRepository historialCalificacionRepository,
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
            _iaService = iaService;
            _historialCalificacionRepository = historialCalificacionRepository;
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
                _logger.LogInformation($"📊 Obteniendo calificaciones para ApplicationUserId: {applicationUserId}");

                // ✅ Obtener el estudiante por ApplicationUserId
                var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(applicationUserId);
                if (estudiante == null)
                {
                    return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse(
                        "Estudiante no encontrado");
                }

                // ✅ Ahora usamos el Id numérico del estudiante
                return await GetCalificacionesPorEstudianteIdAsync(estudiante.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al obtener calificaciones del estudiante con ApplicationUserId {applicationUserId}");
                return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse(
                    "Error al obtener calificaciones",
                    new List<string> { ex.Message });
            }
        }

        // ==================== MÉTODO CORREGIDO: GetCalificacionesPorEstudianteIdAsync ====================
        public async Task<ApiResponse<List<CalificacionEstudianteViewDto>>> GetCalificacionesPorEstudianteIdAsync(int estudianteId)
        {
            try
            {
                _logger.LogInformation($"📊 Obteniendo calificaciones del estudiante ID: {estudianteId}");

                // Verificar que el estudiante existe
                var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId);
                if (estudiante == null)
                {
                    return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse(
                        "Estudiante no encontrado");
                }

                // Obtener todas las calificaciones del estudiante
                var calificaciones = await _calificacionRepository.GetCalificacionesPorEstudianteAsync(estudianteId);

                if (!calificaciones.Any())
                {
                    return ApiResponse<List<CalificacionEstudianteViewDto>>.SuccessResponse(
                        new List<CalificacionEstudianteViewDto>(),
                        "No hay calificaciones registradas");
                }

                // Agrupar por asignatura
                var calificacionesPorAsignatura = calificaciones
                    .GroupBy(c => new { c.IdAsignatura, c.Asignatura.Nombre, c.Asignatura.TipoAsignatura })
                    .Select(g => new CalificacionEstudianteViewDto
                    {
                        IdCalificacion = g.First().Id,
                        IdAsignatura = g.Key.IdAsignatura,
                        AsignaturaNombre = g.Key.Nombre,
                        TipoAsignatura = g.Key.TipoAsignatura,
                        Periodos = g.Select(c => new PeriodoCalificacionViewDto
                        {
                            NumeroPeriodo = c.Periodo.Numero,
                            Componentes = c.Detalles.ToDictionary(
                                d => d.Componente.Nombre,
                                d => (decimal?)d.Valor
                            ),
                            Total = c.Total,
                            Publicada = c.Publicada
                        }).OrderBy(p => p.NumeroPeriodo).ToList(),
                        PromedioGeneral = g.Where(c => c.Publicada).Any()
                            ? g.Where(c => c.Publicada).Average(c => c.Total)
                            : 0
                    })
                    .OrderBy(c => c.AsignaturaNombre)
                    .ToList();

                _logger.LogInformation($"✅ {calificacionesPorAsignatura.Count} asignaturas con calificaciones obtenidas");

                return ApiResponse<List<CalificacionEstudianteViewDto>>.SuccessResponse(
                    calificacionesPorAsignatura,
                    "Calificaciones obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al obtener calificaciones del estudiante {estudianteId}");
                return ApiResponse<List<CalificacionEstudianteViewDto>>.ErrorResponse(
                    "Error al obtener calificaciones",
                    new List<string> { ex.Message });
            }
        }

        // ==================== MÉTODO CORREGIDO: BuscarEstudiantesAsync ====================
        public async Task<ApiResponse<List<EstudianteBusquedaDto>>> BuscarEstudiantesAsync(
            string applicationUserId,
            string userRole,
            string searchTerm,
            int? idGrado,
            int? idCursoAcademico)
        {
            try
            {
                _logger.LogInformation($"🔍 Buscando estudiantes - Rol: {userRole}, Término: '{searchTerm}'");

                // Obtener todas las inscripciones activas con sus relaciones
                var inscripcionesQuery = await _inscripcionRepository.GetAllByConditionAsync(i => i.Estado == "Activa");

                // Si es profesor, filtrar solo sus estudiantes
                if (userRole == "Profesor")
                {
                    var profesor = await _profesorRepository.GetByApplicationUserIdAsync(applicationUserId);
                    if (profesor == null)
                    {
                        return ApiResponse<List<EstudianteBusquedaDto>>.ErrorResponse(
                            "Profesor no encontrado");
                    }

                    // Obtener cursos donde el profesor tiene clases
                    var clases = await _claseProgramadaRepository.GetByProfesorAsync(profesor.Id);
                    var cursosIds = clases.Select(c => c.IdCursoAcademico).Distinct().ToList();

                    inscripcionesQuery = inscripcionesQuery
                        .Where(i => cursosIds.Contains(i.IdCursoAcademico))
                        .ToList();
                }

                // ✅ CORRECCIÓN: Obtener los usuarios mediante UserManager
                var estudiantesConUsuarios = new List<EstudianteBusquedaDto>();

                foreach (var inscripcion in inscripcionesQuery)
                {
                    var user = await _userManager.FindByIdAsync(inscripcion.Estudiante.ApplicationUserId);

                    if (user == null) continue;

                    // Aplicar filtros de búsqueda
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        var termLower = searchTerm.ToLower().Trim();
                        var nombreCompleto = $"{user.FirstName} {user.LastName}".ToLower();
                        var matricula = inscripcion.Estudiante.Matricula.ToLower();

                        if (!nombreCompleto.Contains(termLower) && !matricula.Contains(termLower))
                        {
                            continue;
                        }
                    }

                    // Aplicar filtros adicionales
                    if (idGrado.HasValue && inscripcion.CursoAcademico.IdGrado != idGrado.Value)
                        continue;

                    if (idCursoAcademico.HasValue && inscripcion.IdCursoAcademico != idCursoAcademico.Value)
                        continue;

                    estudiantesConUsuarios.Add(new EstudianteBusquedaDto
                    {
                        Id = inscripcion.IdEstudiante,
                        NombreCompleto = $"{user.FirstName} {user.LastName}",
                        Matricula = inscripcion.Estudiante.Matricula,
                        Email = user.Email,
                        Grado = inscripcion.CursoAcademico.Grado.GradeName,
                        Seccion = inscripcion.CursoAcademico.Seccion.Nombre,
                        IdCursoAcademico = inscripcion.IdCursoAcademico,
                        CursoAcademico = inscripcion.CursoAcademico.NombreCompleto
                    });
                }

                var resultado = estudiantesConUsuarios
                    .OrderBy(e => e.NombreCompleto)
                    .ToList();

                _logger.LogInformation($"✅ {resultado.Count} estudiantes encontrados");

                return ApiResponse<List<EstudianteBusquedaDto>>.SuccessResponse(
                    resultado,
                    $"{resultado.Count} estudiantes encontrados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al buscar estudiantes");
                return ApiResponse<List<EstudianteBusquedaDto>>.ErrorResponse(
                    "Error al buscar estudiantes",
                    new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> EditarCalificacionAsync(
    EditarCalificacionDto dto,
    string userId,
    string userName,
    string userRole)
        {
            try
            {
                _logger.LogInformation($"✏️ Editando calificación - Estudiante: {dto.IdEstudiante}, Asignatura: {dto.IdAsignatura}, Período: {dto.IdPeriodo}");

                // ✅ Obtener la calificación usando los IDs del DTO
                var calificacion = await _calificacionRepository.GetCalificacionConDetallesAsync(
                    dto.IdEstudiante,
                    dto.IdAsignatura,
                    dto.IdPeriodo);

                if (calificacion == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Calificación no encontrada");
                }

                if (!calificacion.Publicada)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "No se puede editar una calificación que no ha sido publicada");
                }

                // Guardar valores anteriores para el historial
                var valoresAnteriores = new ValoresCalificacionDto
                {
                    Componentes = calificacion.Detalles.ToDictionary(
                        d => d.Componente.Nombre,
                        d => (decimal?)d.Valor
                    ),
                    Total = calificacion.Total
                };

                // Obtener componentes de la asignatura
                var componentes = await _componenteRepository.GetByTipoAsignaturaAsync(
                    calificacion.Asignatura.TipoAsignatura);

                // Actualizar los componentes
                decimal nuevoTotal = 0;
                foreach (var componenteDto in dto.Componentes)
                {
                    var componente = componentes.FirstOrDefault(c => c.Nombre == componenteDto.Key);
                    if (componente == null) continue;

                    var valor = componenteDto.Value ?? 0;

                    // Validar que no exceda el máximo
                    if (valor > componente.ValorMaximo)
                    {
                        return ApiResponse<bool>.ErrorResponse(
                            $"El valor de '{componente.Nombre}' ({valor}) excede el máximo ({componente.ValorMaximo})");
                    }

                    // Buscar o crear el detalle
                    var detalle = calificacion.Detalles
                        .FirstOrDefault(d => d.IdComponenteCalificacion == componente.Id);

                    if (detalle != null)
                    {
                        detalle.Valor = valor;
                    }
                    else
                    {
                        var nuevoDetalle = new CalificacionDetalle
                        {
                            IdCalificacion = calificacion.Id,
                            IdComponenteCalificacion = componente.Id,
                            Valor = valor
                        };
                        calificacion.Detalles.Add(nuevoDetalle);
                    }

                    nuevoTotal += valor;
                }

                // Validar que el total no exceda 100
                if (nuevoTotal > 100)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"El total ({nuevoTotal} pts) excede 100 puntos");
                }

                // Actualizar el total
                calificacion.Total = nuevoTotal;
                calificacion.FechaUltimaModificacion = DateTime.Now;

                // Guardar valores nuevos
                var valoresNuevos = new ValoresCalificacionDto
                {
                    Componentes = dto.Componentes,
                    Total = nuevoTotal
                };

                // Crear registro de historial
                var historial = new HistorialCalificacion
                {
                    IdCalificacion = calificacion.Id,
                    NumeroPeriodo = calificacion.Periodo.Numero,
                    ValoresAnteriores = JsonSerializer.Serialize(valoresAnteriores),
                    ValoresNuevos = JsonSerializer.Serialize(valoresNuevos),
                    UsuarioId = userId,
                    UsuarioNombre = userName,
                    UsuarioRol = userRole,
                    MotivoEdicion = dto.MotivoEdicion,
                    FechaModificacion = DateTime.Now
                };

                await _historialCalificacionRepository.AddAsync(historial);

                // Actualizar la calificación
                await _calificacionRepository.UpdateAsync(calificacion);

                _logger.LogInformation($"✅ Calificación editada exitosamente");

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Calificación actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al editar calificación");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al editar la calificación",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> GenerarMensajeIAAsync(int estudianteId, int asignaturaId, int periodoId)
        {
            try
            {
                _logger.LogInformation($"🤖 Generando mensaje IA para estudiante {estudianteId}");

                // Obtener calificación
                var calificacion = await _calificacionRepository.GetCalificacionConDetallesAsync(
                    estudianteId, asignaturaId, periodoId);

                if (calificacion == null || !calificacion.Publicada)
                {
                    return ApiResponse<string>.ErrorResponse("Calificación no encontrada o no publicada");
                }

                // Obtener datos del estudiante
                var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId);
                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                var nombreCompleto = $"{user.FirstName} {user.LastName}";

                // Preparar componentes
                var componentes = calificacion.Detalles.ToDictionary(
                    d => d.Componente.Nombre,
                    d => d.Valor
                );

                // Generar mensaje con IA
                var mensajeIA = await _iaService.GenerarMensajeInicialAsync(
                    nombreCompleto,
                    calificacion.Asignatura.Nombre,
                    calificacion.Asignatura.TipoAsignatura,
                    componentes,
                    calificacion.Total
                );

                _logger.LogInformation("✅ Mensaje IA generado exitosamente");

                return ApiResponse<string>.SuccessResponse(mensajeIA, "Mensaje generado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al generar mensaje IA");
                return ApiResponse<string>.ErrorResponse(
                    "Error al generar mensaje",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> ResponderMensajeIAAsync(
            int estudianteId,
            int asignaturaId,
            string mensajeEstudiante,
            List<string> historial)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId);
                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);
                var nombreCompleto = $"{user.FirstName} {user.LastName}";

                var asignatura = await _asignaturaRepository.GetByIdAsync(asignaturaId);

                var respuesta = await _iaService.ResponderEstudianteAsync(
                    nombreCompleto,
                    asignatura.Nombre,
                    mensajeEstudiante,
                    historial
                );

                return ApiResponse<string>.SuccessResponse(respuesta, "Respuesta generada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al responder mensaje");
                return ApiResponse<string>.ErrorResponse(
                    "Error al procesar mensaje",
                    new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<List<HistorialCalificacionDto>>> GetHistorialCalificacionAsync(int idCalificacion)
        {
            try
            {
                _logger.LogInformation($"📜 Obteniendo historial de calificación {idCalificacion}");

                var historial = await _historialCalificacionRepository.GetByCalificacionIdAsync(idCalificacion);

                var resultado = historial.Select(h => new HistorialCalificacionDto
                {
                    Id = h.Id,
                    IdCalificacion = h.IdCalificacion,
                    NumeroPeriodo = h.NumeroPeriodo,
                    ValoresAnteriores = h.ValoresAnteriores,
                    ValoresNuevos = h.ValoresNuevos,
                    UsuarioNombre = h.UsuarioNombre,
                    UsuarioRol = h.UsuarioRol,
                    FechaModificacion = h.FechaModificacion,
                    MotivoEdicion = h.MotivoEdicion,
                    CambiosRealizados = h.CambiosRealizados
                }).ToList();

                _logger.LogInformation($"✅ {resultado.Count} registros de historial encontrados");

                return ApiResponse<List<HistorialCalificacionDto>>.SuccessResponse(
                    resultado,
                    "Historial obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al obtener historial de calificación {idCalificacion}");
                return ApiResponse<List<HistorialCalificacionDto>>.ErrorResponse(
                    "Error al obtener el historial",
                    new List<string> { ex.Message });
            }
        }
    }
}
