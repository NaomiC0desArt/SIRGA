using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.CursoAcademico;
using SIRGA.Web.Models.Grado;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CursoAcademicoController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CursoAcademicoController> _logger;

        public CursoAcademicoController(ApiService apiService, ILogger<CursoAcademicoController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<CursoAcademicoDto>>>("api/CursoAcademico/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los cursos académicos";
                    return View(new List<CursoAcademicoDto>());
                }

                return View(response.Data ?? new List<CursoAcademicoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cursos académicos");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<CursoAcademicoDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarGrados();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CreateCursoAcademicoDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarGrados();
                return View(model);
            }

            try
            {
                var response = await _apiService.PostAsync<CreateCursoAcademicoDto, ApiResponse<CursoAcademicoDto>>(
                    "api/CursoAcademico/Crear", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "✅ Curso académico creado exitosamente";
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
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear el curso académico");
                }

                await CargarGrados();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear curso académico");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarGrados();
                return View(model);
            }
        }

        // ==================== EDITAR ====================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CursoAcademicoDto>>($"api/CursoAcademico/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Curso académico no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateCursoAcademicoDto
                {
                    IdGrado = response.Data.IdGrado,
                    SchoolYear = response.Data.SchoolYear
                };

                await CargarGrados();
                ViewBag.CursoAcademicoId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar curso académico para editar");
                TempData["ErrorMessage"] = "Error al cargar el curso académico";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, UpdateCursoAcademicoDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarGrados();
                ViewBag.CursoAcademicoId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync($"api/CursoAcademico/Actualizar/{id}", model);

                if (response)
                {
                    TempData["SuccessMessage"] = "✅ Curso académico actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar el curso académico";
                await CargarGrados();
                ViewBag.CursoAcademicoId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar curso académico");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarGrados();
                ViewBag.CursoAcademicoId = id;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CursoAcademicoDto>>($"api/CursoAcademico/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Curso académico no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles de curso académico");
                TempData["ErrorMessage"] = "Error al cargar los detalles";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/CursoAcademico/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "🗑️ Curso académico eliminado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "❌ Error al eliminar el curso académico";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar curso académico");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarGrados()
        {
            try
            {
                var gradosResponse = await _apiService.GetAsync<ApiResponse<List<GradoDto>>>("api/Grado/GetAll");
                ViewBag.Grados = gradosResponse?.Data ?? new List<GradoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar grados");
                ViewBag.Grados = new List<GradoDto>();
            }
        }
    }
}
