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

        /// <summary>
        /// Vista principal - Muestra las clases del día
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fecha)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Today;

                // Obtener el profesor actual directamente desde el endpoint
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

        /// <summary>
        /// Vista para tomar asistencia de una clase específica
        /// </summary>
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

        /// <summary>
        /// Registra la asistencia masiva
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAsistenciaMasiva([FromBody] RegistrarAsistenciaMasivaDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _apiService.PostAsync<RegistrarAsistenciaMasivaDto, ApiResponse<List<AsistenciaResponseDto>>>(
                    "api/Asistencia/Registrar-Masiva", model);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "Asistencia registrada exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al registrar la asistencia",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar asistencia masiva");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Registra asistencia individual
        /// </summary>
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

        /// <summary>
        /// Justifica una asistencia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JustificarAsistencia(int id, [FromBody] JustificarAsistenciaDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Usar el método PatchAsync con body
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

        /// <summary>
        /// Muestra el historial de asistencia de una clase
        /// </summary>
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
