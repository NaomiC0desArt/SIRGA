using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Web.Models.API;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    public class AulaController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AulaController> _logger;

        public AulaController(ApiService apiService, ILogger<AulaController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AulaDto>>>("api/Aula/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las aulas";
                    return View(new List<AulaDto>());
                }

                return View(response.Data ?? new List<AulaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar aulas");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<AulaDto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CreateAulaDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var response = await _apiService.PostAsync<CreateAulaDto, ApiResponse<AulaDto>>(
                    "api/Aula/Crear", model);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "Aula creada exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al crear el aula",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear aula");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AulaDto>>($"api/Aula/{id}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Aula no encontrada" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de aula");
                return Json(new { success = false, message = "Error al cargar el aula" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [FromBody] UpdateAulaDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var response = await _apiService.PutAsync($"api/Aula/Actualizar/{id}", model);

                if (response)
                {
                    return Json(new { success = true, message = "Aula actualizada exitosamente" });
                }

                return Json(new { success = false, message = "Error al actualizar el aula" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar aula");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/Aula/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Aula eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar el aula";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar aula");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GenerarCodigo(int tipo, string nombre)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<string>>(
                    $"api/Aula/GenerarCodigo?tipo={tipo}&nombre={Uri.EscapeDataString(nombre ?? "")}");

                if (response?.Success == true)
                {
                    return Json(new { success = true, codigo = response.Data });
                }

                return Json(new { success = false, message = "Error al generar código" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar código");
                return Json(new { success = false, message = "Error al generar código" });
            }
        }
    }
}
