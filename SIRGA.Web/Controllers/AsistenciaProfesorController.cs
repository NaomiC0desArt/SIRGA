using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Asistencia;
using SIRGA.Web.Services;
using System.Security.Claims;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class AsistenciaProfesorController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AsistenciaProfesorController> _logger;

        public AsistenciaProfesorController(ApiService apiService, ILogger<AsistenciaProfesorController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fecha)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Today;

                var profesorResponse = await _apiService.GetAsync<ApiResponse<ProfesorDto>>("api/Profesor/Current");

                if (profesorResponse?.Success != true || profesorResponse.Data == null)
                {
                    TempData["ErrorMessage"] = "No se pudo identificar al profesor. Por favor, inicia sesión nuevamente.";
                    return View(new List<ClaseDelDiaDto>());
                }

                var profesor = profesorResponse.Data;

                var response = await _apiService.GetAsync<ApiResponse<List<ClaseDelDiaDto>>>(
                    $"api/Asistencia/Profesor/{profesor.Id}/Clases-del-Dia?fecha={fechaConsulta:yyyy-MM-dd}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las clases";
                    return View(new List<ClaseDelDiaDto>());
                }

                ViewBag.FechaConsulta = fechaConsulta;
                ViewBag.ProfesorId = profesor.Id;

                return View(response.Data ?? new List<ClaseDelDiaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar clases del día");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<ClaseDelDiaDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> TomarAsistencia(int idClase, DateTime? fecha)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Today;

                var response = await _apiService.GetAsync<ApiResponse<List<EstudianteClaseDto>>>(
                    $"api/Asistencia/Clase/{idClase}/Estudiantes?fecha={fechaConsulta:yyyy-MM-dd}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los estudiantes";
                    return RedirectToAction(nameof(Index));
                }

                // ✅ NUEVO: Verificar si ya está registrada
                var estudiantesConAsistencia = response.Data?.Where(e => e.YaRegistrada).ToList();
                if (estudiantesConAsistencia?.Any() == true)
                {
                    _logger.LogInformation($"🔒 Asistencia ya registrada para clase {idClase} en fecha {fechaConsulta:yyyy-MM-dd}");
                    TempData["InfoMessage"] = "Esta asistencia ya fue registrada. Solo puedes visualizarla.";
                }

                ViewBag.IdClase = idClase;
                ViewBag.Fecha = fechaConsulta;

                return View(response.Data ?? new List<EstudianteClaseDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estudiantes para asistencia");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAsistenciaMasiva([FromBody] RegistrarAsistenciaMasivaDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("❌ Modelo inválido al registrar asistencia masiva");
                return BadRequest(new { success = false, message = "Datos inválidos", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                // ✅ VALIDACIÓN: Verificar si ya existe asistencia registrada
                var estudiantesResponse = await _apiService.GetAsync<ApiResponse<List<EstudianteClaseDto>>>(
                    $"api/Asistencia/Clase/{model.IdClaseProgramada}/Estudiantes?fecha={model.Fecha:yyyy-MM-dd}");

                if (estudiantesResponse?.Success == true && estudiantesResponse.Data?.Any(e => e.YaRegistrada) == true)
                {
                    _logger.LogWarning($"Intento de duplicar asistencia para clase {model.IdClaseProgramada} en {model.Fecha:yyyy-MM-dd}");
                    return Json(new
                    {
                        success = false,
                        message = "La asistencia ya fue registrada para esta clase. No se permite duplicar registros."
                    });
                }

                _logger.LogInformation($"Registrando asistencia masiva - Clase: {model.IdClaseProgramada}, Fecha: {model.Fecha:yyyy-MM-dd}, Total estudiantes: {model.Asistencias.Count}");

                // Contar justificaciones
                var conJustificacion = model.Asistencias.Count(a => !string.IsNullOrEmpty(a.Justificacion));
                _logger.LogInformation($"📝 Asistencias con justificación: {conJustificacion}");

                var response = await _apiService.PostAsync<RegistrarAsistenciaMasivaDto, ApiResponse<List<AsistenciaResponseDto>>>(
                    "api/Asistencia/Registrar-Masiva", model);

                if (response?.Success == true)
                {
                    _logger.LogInformation($"✅ Asistencia masiva registrada exitosamente para clase {model.IdClaseProgramada}");
                    return Json(new { success = true, message = "Asistencia registrada exitosamente" });
                }

                _logger.LogWarning($"❌ Error al registrar asistencia: {response?.Message}");
                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al registrar la asistencia",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Excepción al registrar asistencia masiva");
                return Json(new { success = false, message = "Error al procesar la solicitud: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAsistencia([FromBody] RegistrarAsistenciaDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _apiService.PostAsync<RegistrarAsistenciaDto, ApiResponse<AsistenciaResponseDto>>(
                    "api/Asistencia/Registrar", model);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "Asistencia registrada exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al registrar la asistencia"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar asistencia");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JustificarAsistencia(int id, [FromBody] JustificarAsistenciaDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _apiService.PatchAsync($"api/Asistencia/{id}/Justificar", model);

                if (response)
                {
                    return Json(new { success = true, message = "Asistencia justificada exitosamente" });
                }

                return Json(new { success = false, message = "Error al justificar la asistencia" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al justificar asistencia");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Historial(int idClase, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var inicio = fechaInicio ?? DateTime.Today.AddMonths(-1);
                var fin = fechaFin ?? DateTime.Today;

                var response = await _apiService.GetAsync<ApiResponse<List<AsistenciaResponseDto>>>(
                    $"api/Asistencia/Clase/{idClase}/Historial?fechaInicio={inicio:yyyy-MM-dd}&fechaFin={fin:yyyy-MM-dd}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar el historial";
                    return View(new List<AsistenciaResponseDto>());
                }

                ViewBag.IdClase = idClase;
                ViewBag.FechaInicio = inicio;
                ViewBag.FechaFin = fin;

                return View(response.Data ?? new List<AsistenciaResponseDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar historial de asistencia");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<AsistenciaResponseDto>());
            }
        }
    }
}
