using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers.Calificaciones
{
    public class CalificacionController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CalificacionController> _logger;

        public CalificacionController(ApiService apiService, ILogger<CalificacionController> logger)
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
                var response = await _apiService.GetAsync<ApiResponse<List<CalificacionDto>>>("api/Calificacion/GetAll");
                if(response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las calificaciones";
                    return View(new List<CalificacionDto>());
                }
                return View(response.Data ?? new List<CalificacionDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar calificaciones");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<CalificacionDto>());
            }
        }

        // ==================== DETALLES ====================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View();
        }

        // ==================== CREAR ====================
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // ==================== EDITAR ====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //==================== ELIMINAR ====================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
