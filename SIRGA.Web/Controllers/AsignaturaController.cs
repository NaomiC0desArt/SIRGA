using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Asignatura;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    public class AsignaturaController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AsignaturaController> _logger;

        public AsignaturaController(ApiService apiService, ILogger<AsignaturaController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AsignaturaDto>>>("api/Asignatura/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las asignaturas";
                    return View(new List<AsignaturaDto>());
                }

                return View(response.Data ?? new List<AsignaturaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar asignaturas");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<AsignaturaDto>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CreateAsignaturaDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<CreateAsignaturaDto, ApiResponse<AsignaturaDto>>(
                    "api/Asignatura/Crear", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Asignatura creada exitosamente";
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
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear la asignatura");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear asignatura");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                return View(model);
            }
        }

 
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AsignaturaDto>>($"api/Asignatura/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Asignatura no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateAsignaturaDto
                {
                    Nombre = response.Data.Nombre,
                    Descripcion = response.Data.Descripcion
                };

                ViewBag.AsignaturaId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar asignatura para editar");
                TempData["ErrorMessage"] = "Error al cargar la asignatura";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, UpdateAsignaturaDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AsignaturaId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync($"api/Asignatura/Actualizar/{id}", model);

                if (response)
                {
                    TempData["SuccessMessage"] = "Asignatura actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar la asignatura";
                ViewBag.AsignaturaId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asignatura");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                ViewBag.AsignaturaId = id;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AsignaturaDto>>($"api/Asignatura/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Asignatura no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                var profesoresResponse = await _apiService.GetAsync<ApiResponse<int>>($"api/Asignatura/{id}/profesores-count");
                ViewBag.CantidadProfesores = profesoresResponse?.Data ?? 0;


                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles de asignatura");
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
                var response = await _apiService.DeleteAsync($"api/Asignatura/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Asignatura eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la asignatura";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar asignatura");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
