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

        // ==================== CREAR ====================
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CreateGradoDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<CreateGradoDto, ApiResponse<GradoDto>>(
                    "api/Grado/Crear", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "✅ Grado creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                if (response?.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear el grado");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear grado");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                return View(model);
            }
        }

        // ==================== EDITAR ====================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<GradoDto>>($"api/Grado/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Grado no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateGradoDto
                {
                    GradeName = response.Data.GradeName,
                    Section = response.Data.Section,
                    StudentsLimit = response.Data.StudentsLimit
                };

                ViewBag.GradoId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar grado para editar");
                TempData["ErrorMessage"] = "Error al cargar el grado";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, UpdateGradoDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GradoId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync($"api/Grado/Actualizar/{id}", model);

                if (response)
                {
                    TempData["SuccessMessage"] = "✅ Grado actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar el grado";
                ViewBag.GradoId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar grado");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                ViewBag.GradoId = id;
                return View(model);
            }
        }

        // ==================== DETALLES ====================
        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<GradoDto>>($"api/Grado/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Grado no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles de grado");
                TempData["ErrorMessage"] = "Error al cargar los detalles";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==================== ELIMINAR ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/Grado/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "🗑️ Grado eliminado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "❌ Error al eliminar el grado";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar grado");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
