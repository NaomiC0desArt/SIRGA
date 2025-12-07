

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.IA;
using SIRGA.Application.Interfaces.Services;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Application.Services.IA
{
    public class ActividadRecomendadorService : IActividadRecomendadorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IActividadExtracurricularRepository _actividadRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ActividadRecomendadorService> _logger;

        public ActividadRecomendadorService(
            ApplicationDbContext context,
            IEstudianteRepository estudianteRepository,
            IInscripcionRepository inscripcionRepository,
            IActividadExtracurricularRepository actividadRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<ActividadRecomendadorService> logger)
        {
            _context = context;
            _estudianteRepository = estudianteRepository;
            _inscripcionRepository = inscripcionRepository;
            _actividadRepository = actividadRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<List<ActividadRecomendadaDto>>> RecomendarActividadesAsync(int idEstudiante)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(idEstudiante);
                if (estudiante == null)
                {
                    return ApiResponse<List<ActividadRecomendadaDto>>.ErrorResponse("Estudiante no encontrado");
                }

                // Obtener el curso académico actual del estudiante
                var inscripcion = await _context.Inscripciones
                    .Include(i => i.CursoAcademico)
                    .Where(i => i.IdEstudiante == idEstudiante)
                    .OrderByDescending(i => i.FechaInscripcion)
                    .FirstOrDefaultAsync();

                if (inscripcion == null)
                {
                    return ApiResponse<List<ActividadRecomendadaDto>>.ErrorResponse(
                        "El estudiante no tiene inscripción activa");
                }

                // Obtener compañeros del mismo curso
                var companeroIds = await _context.Inscripciones
                    .Where(i => i.IdCursoAcademico == inscripcion.IdCursoAcademico
                             && i.IdEstudiante != idEstudiante)
                    .Select(i => i.IdEstudiante)
                    .ToListAsync();

                // Obtener actividades en las que ya está inscrito el estudiante
                var actividadesInscritas = await _context.InscripcionesActividades
                    .Where(ia => ia.IdEstudiante == idEstudiante && ia.EstaActiva)
                    .Select(ia => ia.IdActividad)
                    .ToListAsync();

                // Obtener TODAS las actividades activas
                var todasActividades = await _context.ActividadesExtracurriculares
                    .Include(a => a.ProfesorEncargado)
                    .Include(a => a.Inscripciones)
                    .Where(a => a.EstaActiva)
                    .ToListAsync();

                // Filtrar actividades en las que NO está inscrito
                var actividadesDisponibles = todasActividades
                    .Where(a => !actividadesInscritas.Contains(a.Id))
                    .ToList();

                // Calcular estadísticas para cada actividad
                var recomendaciones = new List<ActividadRecomendadaDto>();

                foreach (var actividad in actividadesDisponibles)
                {
                    // Contar cuántos compañeros están en esta actividad
                    var companeroEnActividad = actividad.Inscripciones
                        .Count(i => companeroIds.Contains(i.IdEstudiante) && i.EstaActiva);

                    var totalInscritos = actividad.Inscripciones.Count(i => i.EstaActiva);

                    // Calcular score de recomendación (0-100)
                    int score = CalcularScoreRecomendacion(companeroEnActividad, totalInscritos);

                    // Obtener datos del profesor
                    var profesor = await _context.Profesores
                        .Where(p => p.Id == actividad.IdProfesorEncargado)
                        .FirstOrDefaultAsync();

                    var user = profesor != null
                        ? await _userManager.FindByIdAsync(profesor.ApplicationUserId)
                        : null;

                    var nombreProfesor = user != null
                        ? $"{user.FirstName} {user.LastName}"
                        : "No asignado";

                    recomendaciones.Add(new ActividadRecomendadaDto
                    {
                        IdActividad = actividad.Id,
                        Nombre = actividad.Nombre,
                        Descripcion = actividad.Descripcion,
                        Categoria = actividad.Categoria,
                        Ubicacion = actividad.Ubicacion,
                        ColorTarjeta = actividad.ColorTarjeta,
                        RutaImagen = actividad.RutaImagen,
                        DiaSemana = actividad.DiaSemana,
                        HoraInicio = actividad.HoraInicio,
                        HoraFin = actividad.HoraFin,
                        NombreProfesor = nombreProfesor,
                        CantidadCompañerosInscritos = companeroEnActividad,
                        PorcentajePopularidad = totalInscritos > 0
                            ? Math.Round((double)totalInscritos / companeroIds.Count * 100, 1)
                            : 0,
                        RazonRecomendacion = GenerarRazonRecomendacion(
                            companeroEnActividad, totalInscritos, actividad.Categoria),
                        ScoreRecomendacion = score
                    });
                }

                // Ordenar por score de recomendación (mayor a menor)
                var recomendacionesOrdenadas = recomendaciones
                    .OrderByDescending(r => r.ScoreRecomendacion)
                    .ThenByDescending(r => r.CantidadCompañerosInscritos)
                    .ToList();

                return ApiResponse<List<ActividadRecomendadaDto>>.SuccessResponse(
                    recomendacionesOrdenadas,
                    $"Se encontraron {recomendacionesOrdenadas.Count} actividades recomendadas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar recomendaciones de actividades");
                return ApiResponse<List<ActividadRecomendadaDto>>.ErrorResponse(
                    "Error al generar recomendaciones",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<EstadisticaActividadDto>>> ObtenerEstadisticasActividadesAsync(
            int idCursoAcademico)
        {
            try
            {
                var estadisticas = await _context.ActividadesExtracurriculares
                    .Include(a => a.Inscripciones)
                    .Where(a => a.EstaActiva)
                    .Select(a => new EstadisticaActividadDto
                    {
                        IdActividad = a.Id,
                        NombreActividad = a.Nombre,
                        CantidadInscritos = a.Inscripciones.Count(i => i.EstaActiva),
                        Categoria = a.Categoria,
                        PorcentajeOcupacion = 0 // Se calculará después
                    })
                    .ToListAsync();

                return ApiResponse<List<EstadisticaActividadDto>>.SuccessResponse(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de actividades");
                return ApiResponse<List<EstadisticaActividadDto>>.ErrorResponse(
                    "Error al obtener estadísticas",
                    new List<string> { ex.Message });
            }
        }

        // ==================== MÉTODOS PRIVADOS ====================

        private int CalcularScoreRecomendacion(int companeros, int totalInscritos)
        {
            int score = 0;

            // Factor 1: Popularidad entre compañeros (40 puntos)
            if (companeros > 5) score += 40;
            else if (companeros > 2) score += 30;
            else if (companeros > 0) score += 20;
            else score += 10; // Actividades nuevas también tienen oportunidad

            // Factor 2: Nivel de participación general (30 puntos)
            if (totalInscritos > 20) score += 30;
            else if (totalInscritos > 10) score += 20;
            else if (totalInscritos > 5) score += 15;
            else score += 10;

            // Factor 3: Disponibilidad (30 puntos) - Actividades con menos gente tienen más cupos
            if (totalInscritos < 15) score += 30;
            else if (totalInscritos < 25) score += 20;
            else score += 10;

            return Math.Min(score, 100); // Máximo 100
        }

        private string GenerarRazonRecomendacion(int companeros, int total, string categoria)
        {
            if (companeros > 5)
            {
                return $"Muy popular entre tus compañeros ({companeros} están inscritos)";
            }
            else if (companeros > 0)
            {
                return $"{companeros} de tus compañeros ya están inscritos";
            }
            else if (total > 15)
            {
                return $"Actividad popular con {total} participantes activos";
            }
            else if (total < 5)
            {
                return "¡Oportunidad de ser pionero! Pocos inscritos aún";
            }
            else
            {
                return $"Actividad de {categoria} con buena participación";
            }
        }
    }
}
