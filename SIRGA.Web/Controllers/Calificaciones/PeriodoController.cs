using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers.Calificaciones
{
    public class PeriodoController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<PeriodoController> _logger;

        public PeriodoController(ApiService apiService, ILogger<PeriodoController> logger)
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
                var response = await _apiService.GetAsync<ApiResponse<List<PeriodoDto>>>("api/Periodo/GetAll");
                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los periodos";
                    return View(new List<PeriodoDto>());
                }

                return View(response.Data ?? new List<PeriodoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar periodos");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<PeriodoDto>());
            }
        }

        // ==================== DETALLES ====================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PeriodoDto>>($"api/Periodo/{id}");
                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar el periodo";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del periodo");
                TempData["ErrorMessage"] = "Error de conexion";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==================== CREAR ====================
        public async Task<IActionResult> Create()
        {
            var aniosEscolares = await ObtenerAniosEscolaresAsync();

            ViewBag.AnioEscolares = aniosEscolares;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PeriodoDto dto)
        {
            try
            {
                var response = await _apiService.PostAsync<PeriodoDto, ApiResponse<PeriodoDto>>("api/Periodo/Crear", dto);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Periodo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                if (response?.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear el periodo.");
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el periodo");
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
                var response = await _apiService.GetAsync<ApiResponse<PeriodoDto>>($"api/Periodo/{id}");
                var responseAnio = await _apiService.GetAsync<ApiResponse<AnioEscolarDto>>($"api/AnioEscolar/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar el Periodo";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new PeriodoDto
                {
                    Numero = response.Data.Numero,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    AnioEscolarId = response.Data.AnioEscolarId,
                    //AnioEscolar = $"{responseAnio.Data.AnioInicio}-{responseAnio.Data.AnioFin}"
                };

                var aniosEscolares = await ObtenerAniosEscolaresAsync();

                ViewBag.AnioEscolares = aniosEscolares;

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

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PeriodoDto dto)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                var response = await _apiService.PutAsync($"api/Periodo/Actualizar/{id}", dto);

                if (response)
                {
                    TempData["SuccessMessage"] = "Periodo actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar el periodo.";
                ViewBag.AnioEscolarId = id;
                return View(dto);
            }
            catch
            {
                _logger.LogError("Error al actualizar el periodo");
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
                var response = await _apiService.DeleteAsync($"api/Periodo/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Periodo eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar el periodo.";
                }
            }
            catch
            {
                _logger.LogError("Error al eliminar el periodo");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
            }
                return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> ObtenerAniosEscolaresAsync()
        {
            var aniosEscolares = await _apiService.GetAsync<ApiResponse<List<AnioEscolarDto>>>("api/AnioEscolar/GetAll");

            var response = new List<SelectListItem>();

            if (aniosEscolares.Success)
            {
                var aniosList = (IEnumerable<AnioEscolarDto>)aniosEscolares.Data;
                response.AddRange(aniosList.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.AnioInicio}-{a.AnioFin}"
                }).ToList());
            }

            return response;
        }
    }
}
