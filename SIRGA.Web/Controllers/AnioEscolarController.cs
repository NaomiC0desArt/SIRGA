using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.AnioEscolar;
using SIRGA.Web.Models.API;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnioEscolarController : Controller
    {

        private readonly ApiService _apiService;
        private readonly ILogger<AnioEscolarController> _logger;

        public AnioEscolarController(ApiService apiService, ILogger<AnioEscolarController> logger)
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

        // ==================== OBTENER TODOS (para dropdowns) ====================
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                _logger.LogInformation("Solicitando años escolares desde la API");

                var response = await _apiService.GetAsync<ApiResponse<List<AnioEscolarDto>>>("api/AnioEscolar/GetAll");

                if (response?.Success != true)
                {
                    _logger.LogWarning("No se pudieron obtener los años escolares: {Message}", response?.Message);
                    return Json(new { success = false, message = "Error al cargar años escolares" });
                }

                _logger.LogInformation("Años escolares obtenidos exitosamente: {Count}", response.Data?.Count ?? 0);
                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar años escolares");
                return Json(new { success = false, message = "Error al cargar años escolares" });
            }
        }

        // ==================== CREAR ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] AnioEscolarDto dto)
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
                var response = await _apiService.PostAsync<AnioEscolarDto, ApiResponse<AnioEscolarDto>>(
                    "api/AnioEscolar/Crear", dto);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "Año escolar creado exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al crear el año escolar",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el año escolar");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        // ==================== ACTIVAR/DESACTIVAR ====================
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            try
            {
                // Primero obtenemos el año escolar
                var getResponse = await _apiService.GetAsync<ApiResponse<AnioEscolarDto>>($"api/AnioEscolar/{id}");

                if (getResponse?.Success != true)
                {
                    return Json(new { success = false, message = "Año escolar no encontrado" });
                }

                // Cambiamos el estado
                var dto = getResponse.Data;
                dto.Activo = !dto.Activo;

                // Actualizamos
                var updateResponse = await _apiService.PutAsync($"api/AnioEscolar/Actualizar/{id}", dto);

                if (updateResponse)
                {
                    var mensaje = dto.Activo ? "Año escolar activado" : "Año escolar desactivado";
                    return Json(new { success = true, message = mensaje });
                }

                return Json(new { success = false, message = "Error al cambiar el estado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del año escolar");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        // ==================== OBTENER DETALLE ====================
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AnioEscolarDto>>($"api/AnioEscolar/{id}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Año escolar no encontrado" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle del año escolar");
                return Json(new { success = false, message = "Error al cargar el año escolar" });
            }
        }
    }
}
