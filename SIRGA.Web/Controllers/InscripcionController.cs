using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.CursoAcademico;
using SIRGA.Web.Models.Estudiante;
using SIRGA.Web.Models.Inscripcion;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InscripcionController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<InscripcionController> _logger;

        public InscripcionController(ApiService apiService, ILogger<InscripcionController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<InscripcionDto>>>("api/Inscripcion/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las inscripciones";
                    return View(new List<InscripcionDto>());
                }

                return View(response.Data ?? new List<InscripcionDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<InscripcionDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarDropdowns();

            var model = new CreateInscripcionDto
            {
                FechaInscripcion = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CreateInscripcionDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();
                return View(model);
            }

            try
            {
                var response = await _apiService.PostAsync<CreateInscripcionDto, ApiResponse<InscripcionDto>>(
                    "api/Inscripcion/Crear", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Inscripción creada exitosamente";
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
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear la inscripción");
                }

                await CargarDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear inscripción");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarDropdowns();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<InscripcionDto>>($"api/Inscripcion/{id}");

            if (response?.Success != true)
            {
                TempData["ErrorMessage"] = "Inscripción no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UpdateInscripcionDto
            {
                IdEstudiante = response.Data.IdEstudiante,
                IdCursoAcademico = response.Data.IdCursoAcademico,
                FechaInscripcion = response.Data.FechaInscripcion
            };

            await CargarDropdowns();
            ViewBag.InscripcionId = id;
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, UpdateInscripcionDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();
                ViewBag.InscripcionId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync($"api/Inscripcion/Actualizar/{id}", model);

                if (response)
                {
                    TempData["SuccessMessage"] = "Inscripción actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar la inscripción";
                await CargarDropdowns();
                ViewBag.InscripcionId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar inscripción");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarDropdowns();
                ViewBag.InscripcionId = id;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<InscripcionDto>>($"api/Inscripcion/{id}");

            if (response?.Success != true)
            {
                TempData["ErrorMessage"] = "Inscripción no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/Inscripcion/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Inscripción eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la inscripción";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar inscripción");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==================== MÉTODOS PRIVADOS ====================
        private async Task CargarDropdowns()
        {
            try
            {
                // Cargar Estudiantes activos
                var estudiantesResponse = await _apiService.GetAsync<ApiResponse<List<EstudianteDto>>>("api/Estudiante/GetAll");

                if (estudiantesResponse?.Data != null && estudiantesResponse.Data.Any())
                {
                    var estudiantes = estudiantesResponse.Data
                        .Where(e => e.IsActive)
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .Select(e => new SelectListItem
                        {
                            Value = e.Id.ToString(),
                            Text = $"{e.FirstName} {e.LastName} - {e.Matricula}"
                        })
                        .ToList();

                    ViewBag.Estudiantes = new SelectList(estudiantes, "Value", "Text");
                }
                else
                {
                    ViewBag.Estudiantes = new SelectList(Enumerable.Empty<SelectListItem>());
                }

                // Cargar Cursos Académicos
                var cursosResponse = await _apiService.GetAsync<ApiResponse<List<CursoAcademicoDto>>>("api/CursoAcademico/GetAll");

                if (cursosResponse?.Data != null && cursosResponse.Data.Any())
                {
                    var cursos = cursosResponse.Data
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = $"{(c.Grado != null ? $"{c.Grado.GradeName} {c.Grado.Section}" : "N/A")} - {c.SchoolYear}"
                        })
                        .ToList();

                    ViewBag.CursosAcademicos = new SelectList(cursos, "Value", "Text");
                }
                else
                {
                    ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dropdowns");

                // Valores por defecto en caso de error
                ViewBag.Estudiantes = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        // ==================== API ENDPOINTS OPCIONALES ====================
        [HttpGet]
        public async Task<JsonResult> ObtenerEstudiantesActivos()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<EstudianteDto>>>("api/Estudiante/GetAll");

                if (response?.Data != null && response.Data.Any())
                {
                    var estudiantes = response.Data
                        .Where(e => e.IsActive)
                        .Select(e => new
                        {
                            id = e.Id,
                            text = $"{e.FirstName} {e.LastName} - {e.Matricula}"
                        })
                        .ToList();

                    return Json(estudiantes);
                }

                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estudiantes");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerCursosAcademicos()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<CursoAcademicoDto>>>("api/CursoAcademico/GetAll");

                if (response?.Data != null && response.Data.Any())
                {
                    var cursos = response.Data
                        .Select(c => new
                        {
                            id = c.Id,
                            text = $"{(c.Grado != null ? $"{c.Grado.GradeName} {c.Grado.Section}" : "N/A")} - {c.SchoolYear}"
                        })
                        .ToList();

                    return Json(cursos);
                }

                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cursos académicos");
                return Json(new List<object>());
            }
        }
    }
}
