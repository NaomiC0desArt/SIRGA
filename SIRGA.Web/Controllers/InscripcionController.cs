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

        public InscripcionController(ApiService apiService, ILogger<InscripcionController> _logger)
        {
            _apiService = apiService;
            this._logger = _logger;
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
                IdCursoAcademico = response.Data.IdCursoAcademico,
                FechaInscripcion = response.Data.FechaInscripcion
            };

            await CargarDropdowns();

            // Pasar datos del estudiante al ViewBag para mostrarlos como solo lectura
            ViewBag.InscripcionId = id;
            ViewBag.IdEstudiante = response.Data.IdEstudiante;
            ViewBag.EstudianteNombre = response.Data.EstudianteNombre ?? "N/A";
            ViewBag.EstudianteMatricula = response.Data.EstudianteMatricula ?? "N/A";

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, UpdateInscripcionDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();

                
                var responseReload = await _apiService.GetAsync<ApiResponse<InscripcionDto>>($"api/Inscripcion/{id}");
                ViewBag.InscripcionId = id;
                ViewBag.IdEstudiante = responseReload?.Data?.IdEstudiante ?? 0;
                ViewBag.EstudianteNombre = responseReload?.Data?.EstudianteNombre ?? "N/A";
                ViewBag.EstudianteMatricula = responseReload?.Data?.EstudianteMatricula ?? "N/A";

                return View(model);
            }

            try
            {
                // Obtener el IdEstudiante original
                var inscripcionActual = await _apiService.GetAsync<ApiResponse<InscripcionDto>>($"api/Inscripcion/{id}");

                if (inscripcionActual?.Success != true)
                {
                    TempData["ErrorMessage"] = "No se pudo obtener la inscripción";
                    return RedirectToAction(nameof(Index));
                }

                // Crear el DTO completo con el IdEstudiante original
                var updateDtoCompleto = new
                {
                    IdEstudiante = inscripcionActual.Data.IdEstudiante, 
                    IdCursoAcademico = model.IdCursoAcademico,
                    FechaInscripcion = model.FechaInscripcion
                };

                var response = await _apiService.PutAsync($"api/Inscripcion/Actualizar/{id}", updateDtoCompleto);

                if (response)
                {
                    TempData["SuccessMessage"] = "Inscripción actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar la inscripción";
                await CargarDropdowns();
                ViewBag.InscripcionId = id;
                ViewBag.IdEstudiante = inscripcionActual.Data.IdEstudiante;
                ViewBag.EstudianteNombre = inscripcionActual.Data.EstudianteNombre;
                ViewBag.EstudianteMatricula = inscripcionActual.Data.EstudianteMatricula;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar inscripción");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";

               
                var responseError = await _apiService.GetAsync<ApiResponse<InscripcionDto>>($"api/Inscripcion/{id}");
                await CargarDropdowns();
                ViewBag.InscripcionId = id;
                ViewBag.IdEstudiante = responseError?.Data?.IdEstudiante ?? 0;
                ViewBag.EstudianteNombre = responseError?.Data?.EstudianteNombre ?? "N/A";
                ViewBag.EstudianteMatricula = responseError?.Data?.EstudianteMatricula ?? "N/A";
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

                    // Datos para JavaScript (búsqueda)
                    ViewBag.EstudiantesData = estudiantesResponse.Data
                        .Where(e => e.IsActive)
                        .OrderBy(e => e.FirstName)
                        .Select(e => new
                        {
                            id = e.Id,
                            nombre = $"{e.FirstName} {e.LastName}",
                            matricula = e.Matricula,
                            nombreCompleto = $"{e.FirstName} {e.LastName} - {e.Matricula}"
                        })
                        .ToList();
                }
                else
                {
                    ViewBag.Estudiantes = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewBag.EstudiantesData = new List<object>();
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
                ViewBag.EstudiantesData = new List<object>();
                ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}
