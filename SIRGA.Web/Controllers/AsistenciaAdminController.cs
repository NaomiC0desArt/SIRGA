using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Asistencia;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AsistenciaAdminController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AsistenciaAdminController> _logger;

        public AsistenciaAdminController(ApiService apiService, ILogger<AsistenciaAdminController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Vista principal - Asistencias que requieren justificación
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AsistenciaResponseDto>>>(
                    "api/Asistencia/Requieren-Justificacion");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las asistencias";
                    return View(new List<AsistenciaResponseDto>());
                }

                return View(response.Data ?? new List<AsistenciaResponseDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar asistencias pendientes");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<AsistenciaResponseDto>());
            }
        }

        /// <summary>
        /// Vista para editar una asistencia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AsistenciaResponseDto>>(
                    $"api/Asistencia/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Asistencia no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new ActualizarAsistenciaDto
                {
                    Estado = response.Data.Estado,
                    Observaciones = response.Data.Observaciones,
                    RequiereJustificacion = response.Data.RequiereJustificacion
                };

                ViewBag.AsistenciaId = id;
                ViewBag.AsistenciaData = response.Data;

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar asistencia para editar");
                TempData["ErrorMessage"] = "Error al cargar la asistencia";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Actualiza una asistencia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ActualizarAsistenciaDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AsistenciaId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync(
                    $"api/Asistencia/Actualizar/{id}", model);

                if (response)
                {
                    TempData["SuccessMessage"] = "Asistencia actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar la asistencia";
                ViewBag.AsistenciaId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                ViewBag.AsistenciaId = id;
                return View(model);
            }
        }

        /// <summary>
        /// Vista para justificar una asistencia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Justificar(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AsistenciaResponseDto>>(
                    $"api/Asistencia/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Asistencia no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (response.Data.Estado == "Presente")
                {
                    TempData["ErrorMessage"] = "No se puede justificar una asistencia marcada como 'Presente'";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.AsistenciaId = id;
                ViewBag.AsistenciaData = response.Data;

                return View(new JustificarAsistenciaDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar asistencia para justificar");
                TempData["ErrorMessage"] = "Error al cargar la asistencia";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Justifica una asistencia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Justificar(int id, JustificarAsistenciaDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AsistenciaId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PatchAsync(
                    $"api/Asistencia/{id}/Justificar");

                // Para el PATCH con body, necesitas ajustar tu ApiService
                // Aquí asumo que funciona similar al PUT

                if (response)
                {
                    TempData["SuccessMessage"] = "Asistencia justificada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al justificar la asistencia";
                ViewBag.AsistenciaId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al justificar asistencia");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                ViewBag.AsistenciaId = id;
                return View(model);
            }
        }

        /// <summary>
        /// Elimina una asistencia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/Asistencia/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Asistencia eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la asistencia";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar asistencia");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Vista de detalles de una asistencia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AsistenciaResponseDto>>(
                    $"api/Asistencia/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Asistencia no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles de asistencia");
                TempData["ErrorMessage"] = "Error al cargar los detalles";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
