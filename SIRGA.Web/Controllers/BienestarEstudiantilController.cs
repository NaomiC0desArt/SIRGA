using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.IA;
using SIRGA.Web.Helpers;
using SIRGA.Web.Models.ActividadExtracurricular;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.IA;
using SIRGA.Web.Models.Profile;
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

        [HttpGet]
        public async Task<IActionResult> Recomendaciones()
        {
            try
            {
                var estudianteIdResponse = await _apiService.GetAsync<EstudianteIdResponse>("api/Estudiante/Mi-Id");

                if (estudianteIdResponse == null || estudianteIdResponse.Id == 0)
                {
                    TempData["ErrorMessage"] = "Error al obtener información del estudiante";
                    return View(new List<ActividadRecomendadaDto>());
                }

                int idEstudiante = estudianteIdResponse.Id;
                ViewBag.IdEstudiante = idEstudiante;

                // Obtener recomendaciones de IA
                var response = await _apiService.GetAsync<ApiResponse<List<ActividadRecomendadaDto>>>(
                    $"api/IA/Recomendador/Actividades/{idEstudiante}");

                if (response?.Success == true)
                {
                    // Procesar URLs de imágenes
                    foreach (var actividad in response.Data)
                    {
                        if (!string.IsNullOrEmpty(actividad.RutaImagen))
                        {
                            actividad.RutaImagen = _imageHelper.GetFullImageUrl(actividad.RutaImagen);
                        }
                    }

                    return View(response.Data);
                }

                TempData["ErrorMessage"] = response?.Message ?? "Error al cargar recomendaciones";
                return View(new List<ActividadRecomendadaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar actividades recomendadas");
                TempData["ErrorMessage"] = "Error al cargar las recomendaciones";
                return View(new List<ActividadRecomendadaDto>());
            }
        }

        /// <summary>
        /// NUEVO: Endpoint para el chatbot (llamado desde JavaScript)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ProcesarConsulta([FromBody] ChatbotRequestDto request)
        {
            try
            {
                var response = await _apiService.PostAsync<ChatbotRequestDto, ApiResponse<ChatbotResponseDto>>(
                    "api/IA/Chatbot/Consulta",
                    request);

                if (response?.Success == true)
                {
                    return Ok(response);
                }

                return BadRequest(new { message = "Error al procesar la consulta" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en chatbot");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// NUEVO: Obtiene el resumen académico del estudiante generado por IA
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MiResumen()
        {
            try
            {
                var estudianteIdResponse = await _apiService.GetAsync<dynamic>("api/Estudiante/Mi-Id");

                if (estudianteIdResponse == null)
                {
                    return Json(new { success = false, message = "Error al obtener información del estudiante" });
                }

                int idEstudiante = (int)estudianteIdResponse.id;

                var response = await _apiService.GetAsync<ApiResponse<string>>(
                    $"api/IA/Chatbot/Mi-Resumen?idEstudiante={idEstudiante}");

                if (response?.Success == true)
                {
                    return Json(new { success = true, resumen = response.Data });
                }

                return Json(new { success = false, message = "Error al generar resumen" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar resumen");
                return Json(new { success = false, message = "Error interno" });
            }
        }
    }
}
