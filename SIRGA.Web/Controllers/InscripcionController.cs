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
        public async Task<IActionResult> Index(
    string busqueda = "",
    string filtroCurso = "",
    int pagina = 1,
    int registrosPorPagina = 10)
        {
            try
            {
                // Obtener todas las inscripciones
                var response = await _apiService.GetAsync<ApiResponse<List<InscripcionDto>>>("api/Inscripcion/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las inscripciones";
                    return View(new List<InscripcionDto>());
                }

                var inscripciones = response.Data ?? new List<InscripcionDto>();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    inscripciones = inscripciones.Where(i =>
                        (i.EstudianteNombre != null && i.EstudianteNombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase)) ||
                        (i.EstudianteMatricula != null && i.EstudianteMatricula.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (!string.IsNullOrWhiteSpace(filtroCurso))
                {
                    if (int.TryParse(filtroCurso, out int cursoId))
                    {
                        inscripciones = inscripciones.Where(i => i.IdCursoAcademico == cursoId).ToList();
                    }
                }

                // Calcular paginación
                var totalRegistros = inscripciones.Count;
                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);

                // Asegurar que la página esté en rango válido
                pagina = Math.Max(1, Math.Min(pagina, totalPaginas > 0 ? totalPaginas : 1));

                // Aplicar paginación
                var inscripcionesPaginadas = inscripciones
                    .Skip((pagina - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToList();

                // Obtener cursos disponibles para el filtro
                var cursosResponse = await _apiService.GetAsync<ApiResponse<List<CursoAcademicoDto>>>("api/CursoAcademico/GetAll");
                if (cursosResponse?.Data != null && cursosResponse.Data.Any())
                {
                    var cursos = cursosResponse.Data
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = $"{(c.Grado != null ? $"{c.Grado.GradeName} {c.Grado.GradeName}" : "N/A")} - {c.AnioEscolar}"
                        })
                        .ToList();

                    ViewBag.CursosDisponibles = cursos;
                }

                // Pasar datos de paginación a la vista
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.RegistrosPorPagina = registrosPorPagina;
                ViewBag.Busqueda = busqueda;
                ViewBag.FiltroCurso = filtroCurso;

                return View(inscripcionesPaginadas);
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

                // Cargar Cursos Académicos - AJUSTADO PARA TU DTO
                var cursosResponse = await _apiService.GetAsync<ApiResponse<List<CursoAcademicoDto>>>("api/CursoAcademico/GetAll");

                if (cursosResponse?.Data != null && cursosResponse.Data.Any())
                {
                    var cursos = cursosResponse.Data
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.NombreCompleto ?? BuildCursoNombre(c)
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

                ViewBag.Estudiantes = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.EstudiantesData = new List<object>();
                ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        // Método helper para construir nombres (igual que en ClaseProgramada)
        private string BuildCursoNombre(CursoAcademicoDto curso)
        {
            var gradoNombre = curso.Grado?.GradeName ?? "N/A";
            var seccionNombre = curso.Seccion?.Nombre ?? "N/A";
            var periodo = curso.AnioEscolar?.Periodo ?? "N/A";

            return $"{gradoNombre} - Sección {seccionNombre} ({periodo})";
        }
    }
}
