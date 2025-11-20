using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Estudiante;
using SIRGA.Web.Models.Profesor;
using SIRGA.Web.Services;
using System.Security.Claims;
using SIRGA.Web.Models.ClaseProgramada;
using SIRGA.Web.Models.Inscripcion;
using SIRGA.Web.Helpers;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
            private readonly ApiService _apiService;

            public AdminController(ApiService apiService)
            {
                _apiService = apiService;
            }

        

        #region Dashboard
        public async Task<IActionResult> Index()
            {
            try {
                var userName = User.FindFirstValue(ClaimTypes.Name);

                // Obtener contadores para el dashboard
                var estudiantesResponse = await _apiService.GetAsync<ApiResponse<List<EstudianteDto>>>("api/Estudiante/GetAll");
                var profesoresResponse = await _apiService.GetAsync<ApiResponse<List<ProfesorDto>>>("api/Profesor/GetAll");

                var model = new AdminDashboardViewModel
                {
                    UserName = userName,
                    TotalEstudiantes = estudiantesResponse?.Data?.Count ?? 0,
                    TotalProfesores = profesoresResponse?.Data?.Count ?? 0,
                    EstudiantesActivos = estudiantesResponse?.Data?.Count(e => e.IsActive) ?? 0,
                    ProfesoresActivos = profesoresResponse?.Data?.Count(p => p.IsActive) ?? 0
                };

                return View(model);
            }
            catch(UnauthorizedAccessException)
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Remove("JWTToken");
                TempData["ErrorMessage"] = "Tu sesión ha expirado o el token es inválido. Por favor, inicia sesión de nuevo.";


                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado. Intenta de nuevo.";
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion

        #region Estudiantes
            [HttpGet]
            public async Task<IActionResult> Estudiantes(
            string searchTerm = "",
            string status = "",
            string sortBy = "nombre",
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                // Obtener todos los estudiantes
                var response = await _apiService.GetAsync<ApiResponse<List<EstudianteDto>>>("api/Estudiante/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los estudiantes";
                    return View(new PaginatedList<EstudianteDto>(new List<EstudianteDto>(), 0, 1, pageSize));
                }

                var estudiantes = response.Data ?? new List<EstudianteDto>();

                // Aplicar filtro de búsqueda
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    estudiantes = estudiantes.Where(e =>
                        (e.FirstName != null && e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (e.LastName != null && e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (e.Matricula != null && e.Matricula.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (e.Email != null && e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Aplicar filtro de estado
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == "activo")
                        estudiantes = estudiantes.Where(e => e.IsActive).ToList();
                    else if (status == "inactivo")
                        estudiantes = estudiantes.Where(e => !e.IsActive).ToList();
                    else if (status == "perfil-incompleto")
                        estudiantes = estudiantes.Where(e => e.MustCompleteProfile).ToList();
                    else if (status == "perfil-completo")
                        estudiantes = estudiantes.Where(e => !e.MustCompleteProfile).ToList();
                }

                // Aplicar ordenamiento
                estudiantes = sortBy?.ToLower() switch
                {
                    "nombre" => estudiantes.OrderBy(e => e.FirstName).ToList(),
                    "apellido" => estudiantes.OrderBy(e => e.LastName).ToList(),
                    "matricula" => estudiantes.OrderBy(e => e.Matricula).ToList(),
                    "email" => estudiantes.OrderBy(e => e.Email).ToList(),
                    "fecha-desc" => estudiantes.OrderByDescending(e => e.Id).ToList(),
                    _ => estudiantes.OrderBy(e => e.FirstName).ToList()
                };

                // Calcular paginación
                var totalCount = estudiantes.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                // Aplicar paginación
                var paginatedEstudiantes = estudiantes
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Crear el modelo paginado
                var paginatedList = new PaginatedList<EstudianteDto>(
                    paginatedEstudiantes,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                // Pasar parámetros al ViewBag
                ViewBag.SearchTerm = searchTerm;
                ViewBag.Status = status;
                ViewBag.SortBy = sortBy;
                ViewBag.PageSize = pageSize;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new PaginatedList<EstudianteDto>(new List<EstudianteDto>(), 0, 1, pageSize));
            }
        }
            [HttpPost]
            public async Task<IActionResult> ActivarEstudiante(int id)
        {
            try
            {
                var response = await _apiService.PatchAsync($"api/Estudiante/Activar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Estudiante activado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al activar el estudiante. Intentelo nuevamente :(";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la solicitud :(";
            }

            return RedirectToAction(nameof(Estudiantes));
        }

            [HttpPost]
            public async Task<IActionResult> DesactivarEstudiante(int id)
        {
            try
            {
                var response = await _apiService.PatchAsync($"api/Estudiante/Desactivar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Estudiante desactivado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al desactivar el estudiante";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Estudiantes));
        }

            [HttpGet]
            public IActionResult CrearEstudiante()
            {
            var model = new CreateEstudianteDto
            {
                YearOfEntry = DateTime.Now.Year
            };
            return View();
            }

            [HttpPost]
            public async Task<IActionResult> CrearEstudiante(CreateEstudianteDto model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                try
                {
                    var response = await _apiService.PostAsync<CreateEstudianteDto, ApiResponse<EstudianteResponseDto>>(
                        "api/Estudiante/Crear", model);

                    if (response?.Success == true)
                    {
                        TempData["SuccessMessage"] = "El estudiante fue creado exitosamente";
                    return RedirectToAction(nameof(Estudiantes));
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
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Ocurrió un error al crear el estudiante");
                }
                return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud";
                    return View(model);
                }
            }

            [HttpGet]
            public async Task<IActionResult> EditarEstudiante(int id)
            {
                var response = await _apiService.GetAsync<ApiResponse<EstudianteDto>>($"api/Estudiante/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Estudiante no encontrado";
                    return RedirectToAction(nameof(Estudiantes));
                }

                var updateDto = new UpdateEstudianteDto
                {
                    FirstName = response.Data.FirstName,
                    LastName = response.Data.LastName,
                    Email = response.Data.Email,
                    PhoneNumber = response.Data.PhoneNumber,
                    Province = response.Data.Province,
                    Sector = response.Data.Sector,
                    Address = response.Data.Address,
                    IsActive = response.Data.IsActive
                };

                ViewBag.EstudianteId = id;
                return View(updateDto);
            }

            [HttpPost]
            public async Task<IActionResult> EditarEstudiante(int id, UpdateEstudianteDto model)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.EstudianteId = id;
                    return View(model);
                }

                try
                {
                    var response = await _apiService.PutAsync($"api/Estudiante/Actualizar/{id}", model);

                    if (response)
                    {
                        TempData["SuccessMessage"] = "Estudiante actualizado exitosamente";
                        return RedirectToAction(nameof(Estudiantes));
                    }

                    TempData["ErrorMessage"] = "Ocurrió un error al actualizar el estudiante";
                    ViewBag.EstudianteId = id;
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud";
                    ViewBag.EstudianteId = id;
                    return View(model);
                }
            }

            [HttpPost]
            public async Task<IActionResult> EliminarEstudiante(int id)
            {
                try
                {
                    var response = await _apiService.DeleteAsync($"api/Estudiante/Eliminar/{id}");

                    if (response)
                    {
                        TempData["SuccessMessage"] = "Estudiante eliminado exitosamente";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ocurrió un error al eliminar el estudiante";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud";
                }

                return RedirectToAction(nameof(Estudiantes));
            }

            [HttpGet]
            public async Task<IActionResult> DetallesEstudiante(int id)
            {
                var response = await _apiService.GetAsync<ApiResponse<EstudianteDto>>($"api/Estudiante/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Estudiante no encontrado";
                    return RedirectToAction(nameof(Estudiantes));
                }

                return View(response.Data);
            }
        #endregion

        #region Profesores
        [HttpGet]
        public async Task<IActionResult> Profesores(
        string searchTerm = "",
        string status = "",
        string specialty = "",
        string sortBy = "nombre",
        int pageNumber = 1,
        int pageSize = 10)
        {
            try
            {
                // oObtener todos los profesores
                var response = await _apiService.GetAsync<ApiResponse<List<ProfesorDto>>>("api/Profesor/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Ocurrió un error al cargar los profesores";
                    return View(new PaginatedList<ProfesorDto>(new List<ProfesorDto>(), 0, 1, pageSize));
                }

                var profesores = response.Data ?? new List<ProfesorDto>();

                // filtro busqueda
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    profesores = profesores.Where(p =>
                        (p.FirstName != null && p.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.LastName != null && p.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Email != null && p.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Specialty != null && p.Specialty.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // filtro estado
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == "activo")
                        profesores = profesores.Where(p => p.IsActive).ToList();
                    else if (status == "inactivo")
                        profesores = profesores.Where(p => !p.IsActive).ToList();
                    else if (status == "perfil-completo")
                        profesores = profesores.Where(p => !p.MustCompleteProfile).ToList();
                    else if (status == "perfil-incompleto")
                        profesores = profesores.Where(p => p.MustCompleteProfile).ToList();
                }

                // filtro especialidad
                if (!string.IsNullOrWhiteSpace(specialty))
                {
                    profesores = profesores.Where(p =>
                        p.Specialty != null && p.Specialty.Contains(specialty, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // ordenar
                profesores = sortBy?.ToLower() switch
                {
                    "nombre" => profesores.OrderBy(p => p.FirstName).ToList(),
                    "apellido" => profesores.OrderBy(p => p.LastName).ToList(),
                    "email" => profesores.OrderBy(p => p.Email).ToList(),
                    "especialidad" => profesores.OrderBy(p => p.Specialty).ToList(),
                    "fecha-desc" => profesores.OrderByDescending(p => p.Id).ToList(),
                    _ => profesores.OrderBy(p => p.FirstName).ToList()
                };


                var totalCount = profesores.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));


                var paginatedProfesores = profesores
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                var paginatedList = new PaginatedList<ProfesorDto>(
                    paginatedProfesores,
                    totalCount,
                    pageNumber,
                    pageSize
                );

 
                ViewBag.SearchTerm = searchTerm;
                ViewBag.Status = status;
                ViewBag.Specialty = specialty;
                ViewBag.SortBy = sortBy;
                ViewBag.PageSize = pageSize;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new PaginatedList<ProfesorDto>(new List<ProfesorDto>(), 0, 1, pageSize));
            }
        }


        [HttpPost]
        public async Task<IActionResult> ActivarProfesor(int id)
        {
            try
            {
                var response = await _apiService.PatchAsync($"api/Profesor/Activar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Profesor activado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al activar el profesor";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Profesores));
        }

        [HttpPost]
        public async Task<IActionResult> DesactivarProfesor(int id)
        {
            try
            {
                var response = await _apiService.PatchAsync($"api/Profesor/Desactivar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Profesor desactivado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al desactivar el profesor";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Profesores));
        }

        [HttpGet]
            public IActionResult CrearProfesor()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> CrearProfesor(CreateProfesorDto model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                try
                {
                    var response = await _apiService.PostAsync<CreateProfesorDto, ApiResponse<ProfesorDto>>(
                        "api/Profesor/Crear", model);

                    if (response?.Success == true)
                    {
                        TempData["SuccessMessage"] = "Profesor creado exitosamente";
                        return RedirectToAction(nameof(Profesores));
                    }

                    TempData["ErrorMessage"] = response?.Message ?? "Ocurrió un error al crear el profesor";
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud";
                    return View(model);
                }
            }

            [HttpGet]
            public async Task<IActionResult> EditarProfesor(int id)
            {
                var response = await _apiService.GetAsync<ApiResponse<ProfesorDto>>($"api/Profesor/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Profesor no encontrado";
                    return RedirectToAction(nameof(Profesores));
                }

                var updateDto = new UpdateProfesorDto
                {
                    FirstName = response.Data.FirstName,
                    LastName = response.Data.LastName,
                    Email = response.Data.Email,
                    PhoneNumber = response.Data.PhoneNumber,
                    Province = response.Data.Province,
                    Sector = response.Data.Sector,
                    Address = response.Data.Address,
                    Specialty = response.Data.Specialty,
                    IsActive = response.Data.IsActive
                };

                ViewBag.ProfesorId = id;
                return View(updateDto);
            }

            [HttpPost]
            public async Task<IActionResult> EditarProfesor(int id, UpdateProfesorDto model)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ProfesorId = id;
                    return View(model);
                }

                try
                {
                    var response = await _apiService.PutAsync($"api/Profesor/Actualizar/{id}", model);

                    if (response)
                    {
                        TempData["SuccessMessage"] = "Profesor actualizado exitosamente";
                        return RedirectToAction(nameof(Profesores));
                    }

                    TempData["ErrorMessage"] = "Error al actualizar el profesor";
                    ViewBag.ProfesorId = id;
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud";
                    ViewBag.ProfesorId = id;
                    return View(model);
                }
            }

            [HttpPost]
            public async Task<IActionResult> EliminarProfesor(int id)
            {
                try
                {
                    var response = await _apiService.DeleteAsync($"api/Profesor/Eliminar/{id}");

                    if (response)
                    {
                        TempData["SuccessMessage"] = "Profesor eliminado exitosamente";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ocurrió un erroral eliminar el profesor";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud";
                }

                return RedirectToAction(nameof(Profesores));
            }

            [HttpGet]
            public async Task<IActionResult> DetallesProfesor(int id)
            {
                var response = await _apiService.GetAsync<ApiResponse<ProfesorDto>>($"api/Profesor/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Profesor no encontrado";
                    return RedirectToAction(nameof(Profesores));
                }

                return View(response.Data);
            }
        #endregion

        
    }
    public class AdminDashboardViewModel
        {
            public string UserName { get; set; }
            public int TotalEstudiantes { get; set; }
            public int TotalProfesores { get; set; }
            public int EstudiantesActivos { get; set; }
            public int ProfesoresActivos { get; set; }
        }
    }
