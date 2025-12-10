using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIRGA.Web.Helpers;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Asignatura;
using SIRGA.Web.Models.ClaseProgramada;
using SIRGA.Web.Models.CursoAcademico;
using SIRGA.Web.Models.Profesor;
using SIRGA.Web.Services;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClaseProgramadaController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ClaseProgramadaController> _logger;

        public ClaseProgramadaController(ApiService apiService, ILogger<ClaseProgramadaController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
        string searchTerm = "",
        string weekDay = "",
        string location = "",
        string sortBy = "dia",
        int pageNumber = 1,
        int pageSize = 10)
        {
            try
            {
                // 1. Obtener todas las clases
                var response = await _apiService.GetAsync<ApiResponse<List<ClaseProgramadaDto>>>("api/ClaseProgramada/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las clases programadas";
                    return View(new PaginatedList<ClaseProgramadaDto>(new List<ClaseProgramadaDto>(), 0, 1, pageSize));
                }

                var clases = response.Data ?? new List<ClaseProgramadaDto>();

                // 2. APLICAR BÚSQUEDA
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    clases = clases.Where(c =>
                        (c.AsignaturaNombre != null && c.AsignaturaNombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.ProfesorNombre != null && c.ProfesorNombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.Location != null && c.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.WeekDay != null && c.WeekDay.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // 3. APLICAR FILTRO DE DÍA
                if (!string.IsNullOrWhiteSpace(weekDay))
                {
                    clases = clases.Where(c =>
                        c.WeekDay != null && c.WeekDay.Equals(weekDay, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // 4. APLICAR FILTRO DE UBICACIÓN
                if (!string.IsNullOrWhiteSpace(location))
                {
                    clases = clases.Where(c =>
                        c.Location != null && c.Location.Contains(location, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // 5. APLICAR ORDENAMIENTO
                clases = sortBy?.ToLower() switch
                {
                    "dia" => clases.OrderBy(c => c.WeekDay).ToList(),
                    "hora" => clases.OrderBy(c => c.StartTime).ToList(),
                    "asignatura" => clases.OrderBy(c => c.AsignaturaNombre).ToList(),
                    "profesor" => clases.OrderBy(c => c.ProfesorNombre).ToList(),
                    "ubicacion" => clases.OrderBy(c => c.Location).ToList(),
                    _ => clases.OrderBy(c => c.WeekDay).ThenBy(c => c.StartTime).ToList()
                };

                // 6. CALCULAR PAGINACIÓN
                var totalCount = clases.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                // 7. APLICAR PAGINACIÓN
                var paginatedClases = clases
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 8. CREAR MODELO PAGINADO
                var paginatedList = new PaginatedList<ClaseProgramadaDto>(
                    paginatedClases,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                // 9. PASAR PARÁMETROS A LA VISTA
                ViewBag.SearchTerm = searchTerm;
                ViewBag.WeekDay = weekDay;
                ViewBag.Location = location;
                ViewBag.SortBy = sortBy;
                ViewBag.PageSize = pageSize;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clases programadas");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new PaginatedList<ClaseProgramadaDto>(new List<ClaseProgramadaDto>(), 0, 1, pageSize));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CreateClaseProgramadaDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();
                return View(model);
            }

            try
            {
                var response = await _apiService.PostAsync<CreateClaseProgramadaDto, ApiResponse<ClaseProgramadaDto>>(
                    "api/ClaseProgramada/Crear", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Clase programada creada exitosamente";
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
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear la clase programada");
                }

                await CargarDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear clase programada");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarDropdowns();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<ClaseProgramadaDto>>($"api/ClaseProgramada/{id}");

            if (response?.Success != true)
            {
                TempData["ErrorMessage"] = "Clase programada no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UpdateClaseProgramadaDto
            {
                StartTime = response.Data.StartTime,
                EndTime = response.Data.EndTime,
                WeekDay = response.Data.WeekDay,
                Location = response.Data.Location,
                IdAsignatura = response.Data.IdAsignatura,
                IdProfesor = response.Data.IdProfesor,
                IdCursoAcademico = response.Data.IdCursoAcademico
            };

            await CargarDropdowns();
            ViewBag.ClaseProgramadaId = id;
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, UpdateClaseProgramadaDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();
                ViewBag.ClaseProgramadaId = id;
                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync($"api/ClaseProgramada/Actualizar/{id}", model);

                if (response)
                {
                    TempData["SuccessMessage"] = "Clase programada actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar la clase programada";
                await CargarDropdowns();
                ViewBag.ClaseProgramadaId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar clase programada");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarDropdowns();
                ViewBag.ClaseProgramadaId = id;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<ClaseProgramadaDto>>($"api/ClaseProgramada/{id}");

            if (response?.Success != true)
            {
                TempData["ErrorMessage"] = "Clase programada no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/ClaseProgramada/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Clase programada eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la clase programada";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar clase programada");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task CargarDropdowns()
        {
            try
            {
                // Cargar Asignaturas
                var asignaturasResponse = await _apiService.GetAsync<ApiResponse<List<AsignaturaDto>>>("api/Asignatura/GetAll");
                var asignaturas = asignaturasResponse?.Data;

                if (asignaturas != null && asignaturas.Any())
                {
                    ViewBag.Asignaturas = new SelectList(asignaturas, "Id", "Nombre");
                }
                else
                {
                    ViewBag.Asignaturas = new SelectList(Enumerable.Empty<SelectListItem>());
                }

                // Cargar Profesores activos
                var profesoresResponse = await _apiService.GetAsync<ApiResponse<List<ProfesorDto>>>("api/Profesor/GetAll");

                if (profesoresResponse?.Data != null && profesoresResponse.Data.Any())
                {
                    var profesores = profesoresResponse.Data
                        .Where(p => p.IsActive)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = $"{p.FirstName} {p.LastName}"
                        })
                        .ToList();

                    ViewBag.Profesores = new SelectList(profesores, "Value", "Text");

                    ViewBag.ProfesoresData = profesoresResponse.Data
                        .Where(p => p.IsActive)
                        .Select(p => new
                        {
                            id = p.Id,
                            nombre = $"{p.FirstName} {p.LastName}",
                            especialidad = p.Specialty
                        })
                        .ToList();
                }
                else
                {
                    ViewBag.Profesores = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewBag.ProfesoresData = new List<object>();
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

                    // Datos para JavaScript (ubicación automática)
                    ViewBag.CursosData = cursosResponse.Data
                        .Select(c => new
                        {
                            id = c.Id,
                            ubicacion = BuildUbicacion(c)
                        })
                        .ToList();
                }
                else
                {
                    ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewBag.CursosData = new List<object>();
                }

                // Días de la semana
                var diasSemana = new List<SelectListItem>
        {
            new SelectListItem { Value = "Lunes", Text = "Lunes" },
            new SelectListItem { Value = "Martes", Text = "Martes" },
            new SelectListItem { Value = "Miércoles", Text = "Miércoles" },
            new SelectListItem { Value = "Jueves", Text = "Jueves" },
            new SelectListItem { Value = "Viernes", Text = "Viernes" },
            new SelectListItem { Value = "Sábado", Text = "Sábado" }
        };

                ViewBag.DiasSemana = new SelectList(diasSemana, "Value", "Text");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dropdowns");

                ViewBag.Asignaturas = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.Profesores = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.DiasSemana = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.ProfesoresData = new List<object>();
                ViewBag.CursosData = new List<object>();
            }
        }

        private string BuildCursoNombre(CursoAcademicoDto curso)
        {
            var gradoNombre = curso.Grado?.GradeName ?? "N/A";
            var seccionNombre = curso.Seccion?.Nombre ?? "N/A";
            var periodo = curso.AnioEscolar?.Periodo ?? "N/A";

            return $"{gradoNombre} - Sección {seccionNombre} ({periodo})";
        }

        private string BuildUbicacion(CursoAcademicoDto curso)
        {
            if (curso.AulaBase != null)
            {
                return curso.AulaBase.Nombre;
            }

            if (curso.Grado != null && curso.Seccion != null)
            {
                return $"Aula {curso.Grado.GradeName}-{curso.Seccion.Nombre}";
            }

            return "Aula Principal";
        }
    }
}

