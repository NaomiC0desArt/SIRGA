using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Estudiante;
using SIRGA.Web.Models.Profesor;
using SIRGA.Web.Services;
using System.Security.Claims;

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

            // ==================== DASHBOARD ====================
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

            // ==================== ESTUDIANTES ====================

            [HttpGet]
            public async Task<IActionResult> Estudiantes()
            {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<EstudianteDto>>>("api/Estudiante/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los estudiantes";
                    return View(new List<EstudianteDto>());
                }

                return View(response.Data ?? new List<EstudianteDto>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<EstudianteDto>());
            }
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
                        TempData["SuccessMessage"] = "Estudiante creado exitosamente";
                    TempData["EstudianteInfo"] = $"📧 Email: {response.Data.Email}<br/>🎫 Matrícula: {response.Data.Matricula}<br/>🔑 Contraseña temporal enviada al correo";
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
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Error al crear el estudiante");
                }
                return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud";
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

                    TempData["ErrorMessage"] = "Error al actualizar el estudiante";
                    ViewBag.EstudianteId = id;
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud";
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
                        TempData["ErrorMessage"] = "Error al eliminar el estudiante";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud";
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

            // ==================== PROFESORES ====================

            [HttpGet]
            public async Task<IActionResult> Profesores()
            {
                var response = await _apiService.GetAsync<ApiResponse<List<ProfesorDto>>>("api/Profesor/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Error al cargar los profesores";
                    return View(new List<ProfesorDto>());
                }

                return View(response.Data);
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

                    TempData["ErrorMessage"] = response?.Message ?? "Error al crear el profesor";
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
                        TempData["ErrorMessage"] = "Error al eliminar el profesor";
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
