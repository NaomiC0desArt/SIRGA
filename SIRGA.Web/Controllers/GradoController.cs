using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Grado;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    public class GradoController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<GradoController> _logger;

        public GradoController(ApiService apiService, ILogger<GradoController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // ==================== LISTADO ====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<GradoDto>>>("api/Grado/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los grados";
                    return View(new List<GradoDto>());
                }

                return View(response.Data ?? new List<GradoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar grados");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<GradoDto>());
            }
        }

        // ==================== API ENDPOINTS (para modales) ====================

        /// <summary>
        /// Obtiene un grado por ID (para modal de edición)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerGrado(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<GradoDto>>($"api/Grado/{id}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Grado no encontrado" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener grado {Id}", id);
                return Json(new { success = false, message = "Error al cargar el grado" });
            }
        }

        /// <summary>
        /// Crea un nuevo grado (llamado desde modal)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CreateGradoDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Datos inválidos", errors });
            }

            try
            {
                var response = await _apiService.PostAsync<CreateGradoDto, ApiResponse<GradoDto>>(
                    "api/Grado/Crear", model);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "✅ Grado creado exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al crear el grado",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear grado");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Actualiza un grado (llamado desde modal)
        /// </summary>
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CreateGradoDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Datos inválidos", errors });
            }

            try
            {
                var response = await _apiService.PutAsync($"api/Grado/Actualizar/{id}", model);

                if (response)
                {
                    return Json(new { success = true, message = "✅ Grado actualizado exitosamente" });
                }

                return Json(new { success = false, message = "Error al actualizar el grado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar grado");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Elimina un grado
        /// </summary>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/Grado/Eliminar/{id}");

                if (response)
                {
                    return Json(new { success = true, message = "🗑️ Grado eliminado exitosamente" });
                }

                return Json(new { success = false, message = "❌ Error al eliminar el grado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar grado {Id}", id);
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }
    }
}
