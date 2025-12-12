using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities.Calificacion;
using SIRGA.Application.Interfaces.Entities;
using System.Security.Claims;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;
        private readonly ILogger<CalificacionController> _logger;

        public CalificacionController(
            ICalificacionService calificacionService,
            ILogger<CalificacionController> logger)
        {
            _calificacionService = calificacionService;
            _logger = logger;
        }

        // ==================== PROFESOR ====================

        [Authorize(Roles = "Profesor")]
        [HttpGet("Mis-Asignaturas")]
        public async Task<IActionResult> GetMisAsignaturas()
        {
            try
            {
                // ✅ Obtener el ApplicationUserId (string GUID) del token JWT
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation($"🔑 ApplicationUserId desde token: {applicationUserId}");

                if (string.IsNullOrEmpty(applicationUserId))
                {
                    _logger.LogWarning("⚠️ No se encontró ApplicationUserId en el token");
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var result = await _calificacionService.GetAsignaturasProfesorAsync(applicationUserId);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error al obtener asignaturas: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ {result.Data?.Count ?? 0} asignaturas devueltas");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener asignaturas del profesor");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Profesor")]
        [HttpGet("Estudiantes-Para-Calificar")]
        public async Task<IActionResult> GetEstudiantesParaCalificar(
            [FromQuery] int idAsignatura,
            [FromQuery] int idCursoAcademico)
        {
            try
            {
                // ✅ Obtener el ApplicationUserId (string GUID) del token JWT
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation($"🔑 ApplicationUserId: {applicationUserId}");
                _logger.LogInformation($"📚 IdAsignatura: {idAsignatura}, IdCurso: {idCursoAcademico}");

                if (string.IsNullOrEmpty(applicationUserId))
                {
                    _logger.LogWarning("⚠️ No se encontró ApplicationUserId en el token");
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var result = await _calificacionService.GetEstudiantesParaCalificarAsync(
                    applicationUserId, idAsignatura, idCursoAcademico);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ {result.Data?.Calificaciones?.Count ?? 0} estudiantes devueltos");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener estudiantes para calificar");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Profesor")]
        [HttpPost("Guardar")]
        public async Task<IActionResult> GuardarCalificaciones([FromBody] GuardarCalificacionesRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ ModelState inválido");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"💾 Guardando calificaciones - Asignatura: {dto.IdAsignatura}, Estudiantes: {dto.Calificaciones?.Count ?? 0}");

                var result = await _calificacionService.GuardarCalificacionesAsync(dto);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error al guardar: {result.Message}");
                    return BadRequest(result);
                }

                _logger.LogInformation("✅ Calificaciones guardadas exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar calificaciones");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Profesor")]
        [HttpPost("Publicar")]
        public async Task<IActionResult> PublicarCalificaciones([FromBody] PublicarCalificacionesDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ ModelState inválido");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"📤 Publicando calificaciones - Asignatura: {dto.IdAsignatura}");

                var result = await _calificacionService.PublicarCalificacionesAsync(dto);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error al publicar: {result.Message}");
                    return BadRequest(result);
                }

                _logger.LogInformation("✅ Calificaciones publicadas exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al publicar calificaciones");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // ==================== ESTUDIANTE ====================

        [Authorize(Roles = "Estudiante")]
        [HttpGet("Mis-Calificaciones")]
        public async Task<IActionResult> GetMisCalificaciones()
        {
            try
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation($"🔑 Estudiante ApplicationUserId: {applicationUserId}");

                if (string.IsNullOrEmpty(applicationUserId))
                {
                    _logger.LogWarning("⚠️ No se encontró ApplicationUserId en el token");
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var result = await _calificacionService.GetCalificacionesEstudianteAsync(applicationUserId);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ Calificaciones del estudiante obtenidas correctamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener calificaciones del estudiante");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // ==================== ADMIN ====================

        [Authorize(Roles = "Admin")]
        [HttpPut("Editar")]
        public async Task<IActionResult> EditarCalificacion([FromBody] EditarCalificacionDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ ModelState inválido");
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "Admin";
                var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "Admin";

                _logger.LogInformation($"✏️ Admin {userName} editando calificación - Estudiante: {dto.IdEstudiante}, Asignatura: {dto.IdAsignatura}, Período: {dto.IdPeriodo}");

                var result = await _calificacionService.EditarCalificacionAsync(
                    dto, userId, userName, userRole);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error al editar: {result.Message}");
                    return BadRequest(result);
                }

                _logger.LogInformation("✅ Calificación editada exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al editar calificación");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Historial/{idCalificacion}")]
        public async Task<IActionResult> GetHistorial(int idCalificacion)
        {
            try
            {
                _logger.LogInformation($"📜 Obteniendo historial de calificación {idCalificacion}");

                var result = await _calificacionService.GetHistorialCalificacionAsync(idCalificacion);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ Historial obtenido: {result.Data?.Count ?? 0} registros");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al obtener historial de la calificación {idCalificacion}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Profesor")]
        [HttpGet("Estudiante/{estudianteId}")]
        public async Task<IActionResult> GetCalificacionesEstudiante(int estudianteId)
        {
            try
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                _logger.LogInformation($"🔑 {userRole} consultando calificaciones del estudiante {estudianteId}");

                if (string.IsNullOrEmpty(applicationUserId))
                {
                    _logger.LogWarning("⚠️ No se encontró ApplicationUserId en el token");
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // TODO: Si es profesor, validar que el estudiante esté en sus clases
                // Por ahora permitimos la consulta

                var result = await _calificacionService.GetCalificacionesPorEstudianteIdAsync(estudianteId);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ Calificaciones del estudiante obtenidas correctamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener calificaciones del estudiante");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Profesor")]
        [HttpGet("Buscar-Estudiantes")]
        public async Task<IActionResult> BuscarEstudiantes(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? idGrado = null,
            [FromQuery] int? idCursoAcademico = null)
        {
            try
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                _logger.LogInformation($"🔍 {userRole} buscando estudiantes: '{searchTerm}'");

                if (string.IsNullOrEmpty(applicationUserId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var result = await _calificacionService.BuscarEstudiantesAsync(
                    applicationUserId,
                    userRole,
                    searchTerm,
                    idGrado,
                    idCursoAcademico);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Error: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ {result.Data?.Count ?? 0} estudiantes encontrados");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al buscar estudiantes");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }


        }
    }
}
