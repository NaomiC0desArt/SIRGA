using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities.Calificacion;
using SIRGA.Web.Models.API;
using SIRGA.Web.Services;
using System.Security.Claims;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class CalificacionController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CalificacionController> _logger;

        public CalificacionController(ApiService apiService, ILogger<CalificacionController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("🏠 Cargando pantalla de asignaturas...");

                // ✅ Ya no necesitamos pasar el ID - la API lo toma del token
                var response = await _apiService.GetAsync<ApiResponse<List<AsignaturaProfesorDto>>>(
                    "api/Calificacion/Mis-Asignaturas");

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error en respuesta: {response?.Message}");
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar asignaturas";
                    return View(new List<AsignaturaProfesorDto>());
                }

                _logger.LogInformation($"✅ {response.Data?.Count ?? 0} asignaturas cargadas");
                return View(response.Data ?? new List<AsignaturaProfesorDto>());
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado - redirigiendo a login");
                TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar asignaturas del profesor");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<AsignaturaProfesorDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Calificar(int idAsignatura, int idCurso)
        {
            try
            {
                _logger.LogInformation($"📝 Cargando pantalla de calificación - Asignatura: {idAsignatura}, Curso: {idCurso}");

                // ✅ Ya no necesitamos pasar el ID del profesor - la API lo toma del token
                var response = await _apiService.GetAsync<ApiResponse<CapturaMasivaDto>>(
                    $"api/Calificacion/Estudiantes-Para-Calificar?idAsignatura={idAsignatura}&idCursoAcademico={idCurso}");

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error en respuesta: {response?.Message}");
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar estudiantes";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"✅ {response.Data?.Calificaciones?.Count ?? 0} estudiantes cargados");

                // Obtener datos adicionales para mostrar en la vista
                var asignaturaResponse = await _apiService.GetAsync<ApiResponse<dynamic>>($"api/Asignatura/{idAsignatura}");
                var cursoResponse = await _apiService.GetAsync<ApiResponse<dynamic>>($"api/CursoAcademico/{idCurso}");
                var periodoResponse = await _apiService.GetAsync<ApiResponse<dynamic>>("api/Periodo/Activo");

                ViewBag.AsignaturaNombre = asignaturaResponse?.Data?.nombre;
                ViewBag.TipoAsignatura = asignaturaResponse?.Data?.tipoAsignatura;
                ViewBag.CursoNombre = cursoResponse?.Data != null
                    ? $"{cursoResponse.Data.grado?.gradeName} {cursoResponse.Data.seccion?.nombre}"
                    : "Curso";
                ViewBag.NumeroPeriodo = periodoResponse?.Data?.numero ?? 0;

                _logger.LogInformation($"📚 Asignatura: {ViewBag.AsignaturaNombre}, Curso: {ViewBag.CursoNombre}");

                return View(response.Data);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado - redirigiendo a login");
                TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar pantalla de calificación");
                TempData["ErrorMessage"] = "Error al cargar datos";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar([FromBody] CapturaMasivaDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ ModelState inválido");
                return Json(new { success = false, message = "Datos inválidos" });
            }

            try
            {
                _logger.LogInformation($"💾 Guardando {model.Calificaciones?.Count ?? 0} calificaciones");

                var response = await _apiService.PostAsync<CapturaMasivaDto, ApiResponse<bool>>(
                    "api/Calificacion/Guardar", model);

                if (response?.Success == true)
                {
                    _logger.LogInformation("✅ Calificaciones guardadas exitosamente");
                    return Json(new { success = true, message = "Calificaciones guardadas exitosamente" });
                }

                _logger.LogWarning($"⚠️ Error al guardar: {response?.Message}");
                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al guardar calificaciones"
                });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado durante guardado");
                return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página e inicia sesión nuevamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar calificaciones");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publicar([FromBody] PublicarCalificacionesDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ ModelState inválido");
                return Json(new { success = false, message = "Datos inválidos" });
            }

            try
            {
                _logger.LogInformation($"📤 Publicando calificaciones - Asignatura: {model.IdAsignatura}");

                var response = await _apiService.PostAsync<PublicarCalificacionesDto, ApiResponse<bool>>(
                    "api/Calificacion/Publicar", model);

                if (response?.Success == true)
                {
                    _logger.LogInformation("✅ Calificaciones publicadas exitosamente");
                    return Json(new
                    {
                        success = true,
                        message = response.Message ?? "Calificaciones publicadas exitosamente"
                    });
                }

                _logger.LogWarning($"⚠️ Error al publicar: {response?.Message}");
                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al publicar calificaciones"
                });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado durante publicación");
                return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página e inicia sesión nuevamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al publicar calificaciones");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }
    }
}
