using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Helpers;
using SIRGA.Web.Models.ActividadExtracurricular;
using SIRGA.Web.Models.API;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Estudiante")]
    public class BienestarEstudiantilController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<BienestarEstudiantilController> _logger;
        private readonly ImageUrlHelper _imageHelper;

        public BienestarEstudiantilController(
            ApiService apiService,
            ImageUrlHelper imageHelper,
            ILogger<BienestarEstudiantilController> logger)
        {
            _apiService = apiService;
            _imageHelper = imageHelper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string categoria = "")
        {
            try
            {
                string endpoint = string.IsNullOrWhiteSpace(categoria)
                    ? "api/ActividadExtracurricular/Estudiante/Disponibles"
                    : $"api/ActividadExtracurricular/Estudiante/Por-Categoria/{categoria}";

                var response = await _apiService.GetAsync<ApiResponse<List<ActividadViewModel>>>(endpoint);

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las actividades";
                    return View(new List<ActividadViewModel>());
                }

                var actividades = response.Data ?? new List<ActividadViewModel>();

                // ✅ PROCESAR URLS DE IMÁGENES
                foreach (var actividad in actividades)
                {
                    if (!string.IsNullOrEmpty(actividad.RutaImagen))
                    {
                        actividad.RutaImagen = _imageHelper.GetFullImageUrl(actividad.RutaImagen);
                    }
                }

                ViewBag.CategoriaActual = categoria;
                return View(response.Data ?? new List<ActividadViewModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<ActividadViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<ActividadDetalleViewModel>>(
                    $"api/ActividadExtracurricular/Estudiante/{id}/Detalle");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Actividad no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(response.Data.RutaImagen))
                {
                    response.Data.RutaImagen = _imageHelper.GetFullImageUrl(response.Data.RutaImagen);
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalle de actividad {id}");
                TempData["ErrorMessage"] = "Error al cargar el detalle";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Inscribirse(int id)
        {
            try
            {
                var response = await _apiService.PostAsync<object, ApiResponse<bool>>(
                    $"api/ActividadExtracurricular/Estudiante/{id}/Inscribirse",
                    new { });

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "¡Te has inscrito exitosamente! Esta actividad ahora aparecerá en tu horario.";
                }
                else
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al inscribirse";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al inscribirse en actividad {id}");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Desinscribirse(int id)
        {
            try
            {
                var response = await _apiService.PostAsync<object, ApiResponse<bool>>(
                    $"api/ActividadExtracurricular/Estudiante/{id}/Desinscribirse",
                    new { });

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Te has desinscrito exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al desinscribirse";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al desinscribirse de actividad {id}");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}
