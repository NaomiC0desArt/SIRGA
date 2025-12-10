using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.AnioEscolar;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Periodo;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PeriodoController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<PeriodoController> _logger;

        public PeriodoController(ApiService apiService, ILogger<PeriodoController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // ==================== VISTA PRINCIPAL ====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<PeriodoDto>>>("api/Periodo/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los períodos";
                    return View(new List<PeriodoDto>());
                }

                return View(response.Data ?? new List<PeriodoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar períodos");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<PeriodoDto>());
            }
        }

        // ==================== CREAR PERÍODO ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CreatePeriodoDto model)
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
                var response = await _apiService.PostAsync<CreatePeriodoDto, ApiResponse<PeriodoDto>>(
                    "api/Periodo/Crear", model);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "Período creado exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al crear el período",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear período");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        // ==================== ACTUALIZAR PERÍODO ====================
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CreatePeriodoDto model)
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
                var response = await _apiService.PutAsync($"api/Periodo/Actualizar/{id}", model);

                if (response)
                {
                    return Json(new { success = true, message = "Período actualizado exitosamente" });
                }

                return Json(new { success = false, message = "Error al actualizar el período" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar período");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        // ==================== ELIMINAR PERÍODO ====================
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/Periodo/Eliminar/{id}");

                if (response)
                {
                    return Json(new { success = true, message = "Período eliminado exitosamente" });
                }

                return Json(new { success = false, message = "Error al eliminar el período" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar período {Id}", id);
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        // ==================== OBTENER DETALLE ====================
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PeriodoDto>>($"api/Periodo/{id}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Período no encontrado" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle del período");
                return Json(new { success = false, message = "Error al cargar el período" });
            }
        }

        // ==================== OBTENER POR AÑO ESCOLAR ====================
        [HttpGet]
        public async Task<IActionResult> PorAnioEscolar(int anioEscolarId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<PeriodoDto>>>(
                    $"api/Periodo/PorAnioEscolar/{anioEscolarId}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Error al cargar períodos" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener períodos por año escolar");
                return Json(new { success = false, message = "Error al cargar períodos" });
            }
        }

        // ==================== OBTENER PERÍODO ACTIVO ====================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PeriodoActivo()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PeriodoActivoDto>>("api/Periodo/Activo");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "No hay período activo" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período activo");
                return Json(new { success = false, message = "Error al cargar período activo" });
            }
        }

        // ==================== HELPER: OBTENER TODOS LOS AÑOS ESCOLARES ====================
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AnioEscolarDto>>>("api/AnioEscolar/GetAll");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Error al cargar años escolares" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar años escolares");
                return Json(new { success = false, message = "Error al cargar años escolares" });
            }
        }
    }
}
