using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Asignatura;
using SIRGA.Web.Models.Calificacion;
using SIRGA.Web.Models.CursoAcademico;
using SIRGA.Web.Models.Periodo;
using SIRGA.Web.Models.Profesor;
using SIRGA.Web.Models.Profile;
using SIRGA.Web.Services;
using System.Security.Claims;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class ProfesorController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ProfesorController> _logger;

        public ProfesorController(ApiService apiService, ILogger<ProfesorController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // ==================== DASHBOARD ====================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileResponse = await _apiService.GetAsync<ApiResponse<UserProfileDto>>("api/Profile/My-Profile");

            if (profileResponse?.Data?.MustCompleteProfile == true)
            {
                return RedirectToAction(nameof(CompleteProfile));
            }

            var model = new ProfesorDashboardViewModel
            {
                Profile = profileResponse?.Data,
                UserName = User.FindFirstValue(ClaimTypes.Name)
            };

            return View(model);
        }

        // ==================== PERFIL ====================

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(CompleteTeacherProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<CompleteTeacherProfileDto, ApiResponse<object>>(
                    "api/Profesor/Completar-Perfil", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Perfil completado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response?.Message ?? "Error al completar el perfil";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar perfil");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profileResponse = await _apiService.GetAsync<ApiResponse<UserProfileDto>>("api/Profile/My-Profile");

            if (profileResponse?.Success != true)
            {
                TempData["ErrorMessage"] = "Error al cargar el perfil";
                return RedirectToAction(nameof(Index));
            }

            return View(profileResponse.Data);
        }

        // ==================== CALIFICACIONES ====================

        [HttpGet]
        public async Task<IActionResult> MisAsignaturas()
        {
            try
            {
                _logger.LogInformation("🏠 Cargando asignaturas del profesor...");

                var response = await _apiService.GetAsync<ApiResponse<List<AsignaturaProfesorDto>>>(
                    "api/Calificacion/Mis-Asignaturas");

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error: {response?.Message}");
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar asignaturas";
                    return View("MisAsignaturas", new List<AsignaturaProfesorDto>());
                }

                _logger.LogInformation($"✅ {response.Data?.Count ?? 0} asignaturas cargadas");
                return View("MisAsignaturas", response.Data ?? new List<AsignaturaProfesorDto>());
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado");
                TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar asignaturas");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View("MisAsignaturas", new List<AsignaturaProfesorDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Calificar(int idAsignatura, int idCurso)
        {
            try
            {
                _logger.LogInformation($"📝 Calificar - Asignatura: {idAsignatura}, Curso: {idCurso}");

                var response = await _apiService.GetAsync<ApiResponse<CapturaMasivaDto>>(
                    $"api/Calificacion/Estudiantes-Para-Calificar?idAsignatura={idAsignatura}&idCursoAcademico={idCurso}");

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error: {response?.Message}");
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar estudiantes";
                    return RedirectToAction(nameof(MisAsignaturas));
                }

                _logger.LogInformation($"✅ {response.Data?.Calificaciones?.Count ?? 0} estudiantes");

                // Obtener datos adicionales con manejo de errores
                try
                {
                    var asignaturaResponse = await _apiService.GetAsync<ApiResponse<dynamic>>($"api/Asignatura/{idAsignatura}");
                    var cursoResponse = await _apiService.GetAsync<ApiResponse<dynamic>>($"api/CursoAcademico/{idCurso}");
                    var periodoResponse = await _apiService.GetAsync<ApiResponse<dynamic>>("api/Periodo/Activo");

                    // Usar GetProperty de JsonElement de forma segura
                    ViewBag.AsignaturaNombre = asignaturaResponse?.Data?.GetProperty("nombre").GetString() ?? "Asignatura";
                    ViewBag.TipoAsignatura = asignaturaResponse?.Data?.GetProperty("tipoAsignatura").GetString() ?? "";

                    if (cursoResponse?.Data != null)
                    {
                        var curso = cursoResponse.Data;
                        var gradoNombre = curso.GetProperty("grado").GetProperty("gradeName").GetString();
                        var seccionNombre = curso.GetProperty("seccion").GetProperty("nombre").GetString();
                        ViewBag.CursoNombre = $"{gradoNombre} {seccionNombre}";
                    }
                    else
                    {
                        ViewBag.CursoNombre = "Curso";
                    }

                    ViewBag.NumeroPeriodo = periodoResponse?.Data?.GetProperty("numero").GetInt32() ?? 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener datos adicionales, usando valores por defecto");
                    ViewBag.AsignaturaNombre = "Asignatura";
                    ViewBag.TipoAsignatura = response.Data?.TipoAsignatura ?? "";
                    ViewBag.CursoNombre = "Curso";
                    ViewBag.NumeroPeriodo = 1;
                }

                return View("Calificar", response.Data);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado");
                TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar pantalla de calificación");
                TempData["ErrorMessage"] = "Error al cargar datos";
                return RedirectToAction(nameof(MisAsignaturas));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarCalificaciones([FromBody] CapturaMasivaDto model)
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
                    _logger.LogInformation("✅ Guardadas exitosamente");
                    return Json(new { success = true, message = "Calificaciones guardadas exitosamente" });
                }

                _logger.LogWarning($"⚠️ Error: {response?.Message}");
                return Json(new { success = false, message = response?.Message ?? "Error al guardar" });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublicarCalificaciones([FromBody] PublicarCalificacionesDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos" });
            }

            try
            {
                _logger.LogInformation($"📤 Publicando calificaciones");

                var response = await _apiService.PostAsync<PublicarCalificacionesDto, ApiResponse<bool>>(
                    "api/Calificacion/Publicar", model);

                if (response?.Success == true)
                {
                    _logger.LogInformation("✅ Publicadas exitosamente");
                    return Json(new { success = true, message = response.Message ?? "Publicadas exitosamente" });
                }

                _logger.LogWarning($"⚠️ Error: {response?.Message}");
                return Json(new { success = false, message = response?.Message ?? "Error al publicar" });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al publicar");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }
    }

    public class ProfesorDashboardViewModel
    {
        public UserProfileDto Profile { get; set; }
        public string UserName { get; set; }
    }
}
