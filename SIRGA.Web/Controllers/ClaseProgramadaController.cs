using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<ClaseProgramadaDto>>>("api/ClaseProgramada/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las clases programadas";
                    return View(new List<ClaseProgramadaDto>());
                }

                return View(response.Data ?? new List<ClaseProgramadaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clases programadas");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<ClaseProgramadaDto>());
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

        // ==================== MÉTODOS PRIVADOS ====================
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

                    // Datos para JavaScript (búsqueda)
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

                    // ✨ NUEVO: Datos para JavaScript (ubicación automática)
                    ViewBag.CursosData = cursosResponse.Data
                        .Select(c => new
                        {
                            id = c.Id,
                            ubicacion = c.Grado != null
                                ? $"Aula {c.Grado.GradeName}-{c.Grado.Section}"
                                : "Aula Principal"
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

                // Valores por defecto en caso de error
                ViewBag.Asignaturas = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.Profesores = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.CursosAcademicos = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.DiasSemana = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.ProfesoresData = new List<object>();
                ViewBag.CursosData = new List<object>();
            }
        }
    }
}

