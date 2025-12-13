using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.AnioEscolar;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Calificacion;
using SIRGA.Web.Models.Estudiante;
using SIRGA.Web.Models.Profile;
using SIRGA.Web.Services;
using System.Security.Claims;
using System.Text.Json;

namespace SIRGA.Web.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<EstudianteController> _logger;

        public EstudianteController(ApiService apiService, ILogger<EstudianteController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileResponse = await _apiService.GetAsync<ApiResponse<UserProfileDto>>("api/Profile/My-Profile");

            if (profileResponse?.Data?.MustCompleteProfile == true)
            {
                return RedirectToAction(nameof(CompleteProfile));
            }

            var model = new EstudianteDashboardViewModel
            {
                Profile = profileResponse?.Data,
                UserName = User.FindFirstValue(ClaimTypes.Name)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(CompleteStudentProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<CompleteStudentProfileDto, ApiResponse<object>>(
                    "api/Estudiante/Completar-Perfil", model);

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

        [HttpGet]
        public async Task<IActionResult> Horario()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<HorarioSemanalViewModel>>(
                    "api/Horario/Mi-Horario"
                );

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar el horario";
                    return View(new HorarioSemanalViewModel());
                }

                return View(response.Data);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar horario");
                TempData["ErrorMessage"] = "Error al cargar el horario";
                return View(new HorarioSemanalViewModel());
            }
        }

        // ✅ MÉTODO MODIFICADO: MisCalificaciones con EstudianteId
        [HttpGet]
        public async Task<IActionResult> MisCalificaciones()
        {
            try
            {
                _logger.LogInformation("📊 Cargando calificaciones del estudiante");

                // ✅ Obtener el ApplicationUserId del usuario autenticado
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(applicationUserId))
                {
                    _logger.LogWarning("⚠️ No se encontró ApplicationUserId");
                    TempData["ErrorMessage"] = "No se pudo identificar al usuario";
                    return RedirectToAction("Index");
                }

                _logger.LogInformation($"🔑 ApplicationUserId: {applicationUserId}");

                // ✅ PRIMERO: Obtener el ID numérico del estudiante usando el endpoint existente
                var estudianteIdResponse = await _apiService.GetAsync<ApiResponse<int>>(
                    "api/Estudiante/Mi-Id"
                );

                int estudianteId = 0;
                if (estudianteIdResponse?.Success == true)
                {
                    estudianteId = estudianteIdResponse.Data;
                    _logger.LogInformation($"✅ Estudiante ID numérico: {estudianteId}");
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudo obtener el ID del estudiante");
                }

                // ✅ Obtener las calificaciones
                var response = await _apiService.GetAsync<ApiResponse<List<CalificacionEstudianteViewDto>>>(
                    "api/Calificacion/Mis-Calificaciones");

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error: {response?.Message}");
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar calificaciones";
                    return View(new List<CalificacionEstudianteViewDto>());
                }

                // ✅ Obtener el período activo
                int numeroPeriodo = 1;
                try
                {
                    var periodoResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>("api/Periodo/Activo");
                    if (periodoResponse?.Success == true)
                    {
                        numeroPeriodo = periodoResponse.Data.GetProperty("numero").GetInt32();
                        _logger.LogInformation($"📅 Período activo: {numeroPeriodo}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ No se pudo cargar período activo");
                }

                // ✅ Obtener el año escolar activo
                try
                {
                    var anioResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>("api/AnioEscolar/Activo");

                    if (anioResponse?.Success == true && anioResponse.Data.ValueKind != JsonValueKind.Null)
                    {
                        ViewBag.PeriodoAcademico = anioResponse.Data.GetProperty("periodo").GetString();
                        _logger.LogInformation($"✅ Año escolar: {ViewBag.PeriodoAcademico}");
                    }
                    else
                    {
                        ViewBag.PeriodoAcademico = DateTime.Now.Year.ToString();
                        _logger.LogWarning("⚠️ No se pudo cargar año escolar, usando año actual");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar año escolar");
                    ViewBag.PeriodoAcademico = DateTime.Now.Year.ToString();
                }

                // ✅ CRÍTICO: Pasar el EstudianteId al ViewBag para JavaScript
                ViewBag.EstudianteId = estudianteId;
                ViewBag.NumeroPeriodo = numeroPeriodo;

                _logger.LogInformation($"✅ {response.Data?.Count ?? 0} asignaturas con calificaciones");
                _logger.LogInformation($"✅ EstudianteId pasado a la vista: {estudianteId}");

                return View(response.Data ?? new List<CalificacionEstudianteViewDto>());
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado");
                TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar calificaciones");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<CalificacionEstudianteViewDto>());
            }
        }

        // ==================== MÉTODOS IA ====================

        /// <summary>
        /// Genera mensaje inicial de IA para una calificación
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarMensajeIA([FromBody] GenerarMensajeIARequest request)
        {
            try
            {
                _logger.LogInformation($"🤖 [MVC] Generando mensaje IA - Estudiante: {request.EstudianteId}, Asignatura: {request.AsignaturaId}");

                // ✅ Validar que el estudiante ID no sea 0
                if (request.EstudianteId == 0)
                {
                    _logger.LogWarning("⚠️ EstudianteId es 0 - no se puede generar mensaje");
                    return Json(new
                    {
                        success = false,
                        message = "No se pudo identificar al estudiante. Recarga la página."
                    });
                }

                var response = await _apiService.PostAsync<GenerarMensajeIARequest, ApiResponse<string>>(
                    "api/IACalificacion/Generar-Mensaje",
                    request
                );

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error IA: {response?.Message}");
                    return Json(new
                    {
                        success = false,
                        message = response?.Message ?? "Error al generar mensaje"
                    });
                }

                _logger.LogInformation("✅ Mensaje IA generado correctamente");
                return Json(new
                {
                    success = true,
                    data = response.Data,
                    message = response.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado al generar mensaje IA");
                return Json(new
                {
                    success = false,
                    message = "Tu sesión ha expirado. Recarga la página e inicia sesión nuevamente."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al generar mensaje IA");
                return Json(new
                {
                    success = false,
                    message = "Error de conexión con el servidor de IA"
                });
            }
        }

        /// <summary>
        /// Responde a un mensaje del estudiante con IA
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResponderMensajeIA([FromBody] ResponderMensajeIARequest request)
        {
            try
            {
                _logger.LogInformation($"🤖 [MVC] Respondiendo mensaje IA - Estudiante: {request.EstudianteId}");

                // ✅ Validar que el estudiante ID no sea 0
                if (request.EstudianteId == 0)
                {
                    _logger.LogWarning("⚠️ EstudianteId es 0 - no se puede responder");
                    return Json(new
                    {
                        success = false,
                        message = "No se pudo identificar al estudiante. Recarga la página."
                    });
                }

                var response = await _apiService.PostAsync<ResponderMensajeIARequest, ApiResponse<string>>(
                    "api/IACalificacion/Responder",
                    request
                );

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error IA: {response?.Message}");
                    return Json(new
                    {
                        success = false,
                        message = response?.Message ?? "Error al responder"
                    });
                }

                _logger.LogInformation("✅ Respuesta IA generada correctamente");
                return Json(new
                {
                    success = true,
                    data = response.Data,
                    message = response.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ Token expirado al responder mensaje IA");
                return Json(new
                {
                    success = false,
                    message = "Tu sesión ha expirado. Recarga la página e inicia sesión nuevamente."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al responder mensaje IA");
                return Json(new
                {
                    success = false,
                    message = "Error de conexión con el servidor de IA"
                });
            }
        }

        // ==================== MODELOS IA ====================

        public class GenerarMensajeIARequest
        {
            public int EstudianteId { get; set; }
            public int AsignaturaId { get; set; }
            public int PeriodoId { get; set; }
        }

        public class ResponderMensajeIARequest
        {
            public int EstudianteId { get; set; }
            public int AsignaturaId { get; set; }
            public string MensajeEstudiante { get; set; }
            public List<string> HistorialConversacion { get; set; }
        }
    }

    public class EstudianteDashboardViewModel
    {
        public UserProfileDto Profile { get; set; }
        public string UserName { get; set; }
    }
}
