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

        [HttpGet]
        public async Task<IActionResult> MisCalificaciones()
        {
            try
            {
                _logger.LogInformation("📊 Cargando calificaciones del estudiante...");

                var response = await _apiService.GetAsync<ApiResponse<List<CalificacionEstudianteViewDto>>>(
                    "api/Calificacion/Mis-Calificaciones");

                if (response?.Success != true)
                {
                    _logger.LogWarning($"⚠️ Error: {response?.Message}");
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar calificaciones";
                    return View(new List<CalificacionEstudianteViewDto>());
                }

                // ✅ CORREGIR: Obtener año escolar actual con manejo de JsonElement
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

                _logger.LogInformation($"✅ {response.Data?.Count ?? 0} asignaturas con calificaciones");
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
    }

    public class EstudianteDashboardViewModel
    {
        public UserProfileDto Profile { get; set; }
        public string UserName { get; set; }
    }
}
