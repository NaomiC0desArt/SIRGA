using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers.Calificaciones
{
    public class AnioEscolarController(ApiService apiService, ILogger<ApiService> logger) : Controller
    {
        private readonly ApiService _apiService = apiService;
        private readonly ILogger<ApiService> _logger = logger;

        // ==================== LISTADO ====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AnioEscolarDto>>>("api/AnioEscolar/GetAll");
                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los años escolares";
                    return View(new List<AnioEscolarDto>());
                }
                return View(response.Data ?? new List<AnioEscolarDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar años escolares");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<AnioEscolarDto>());
            }
        }

        // ==================== DETALLES ====================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AnioEscolarDto>>($"api/AnioEscolar/{id}");
                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar el año escolar";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del año escolar");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==================== CREAR ====================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnioEscolarDto dto)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                var response = await _apiService.PostAsync<AnioEscolarDto, ApiResponse<AnioEscolarDto>>(
                    "api/AnioEscolar/Crear", dto);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Año escolar creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                if(response?.Errors != null && response.Errors.Any())
                {
                    foreach(var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear el año escolar.");
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el año escolar");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(dto);
            }
        }

        // ==================== EDITAR ====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AnioEscolarDto>>($"api/AnioEscolar/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar el año escolar";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new AnioEscolarDto
                {
                    AnioInicio = response.Data.AnioInicio,
                    AnioFin = response.Data.AnioFin,
                    Activo = response.Data.Activo
                };
                ViewBag.AnioEscolarId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el año escolar para editar");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnioEscolarDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AnioEscolarId = id;
                return View();
            }

            try
            {
                var response = await _apiService.PutAsync($"api/AnioEscolar/Actualizar/{id}", dto);
                
                if (response)
                {
                    TempData["SuccessMessage"] = "Año escolar actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar el año escolar.";
                ViewBag.AnioEscolarId = id;
                return View(dto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el año escolar");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                ViewBag.AnioEscolarId = id;
                return View(dto);
            }
        }

        // ==================== ELIMINAR ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/AnioEscolar/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Año escolar eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar el año escolar.";
                }
            }
            catch
            {
                _logger.LogError("Error al eliminar el año escolar");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
