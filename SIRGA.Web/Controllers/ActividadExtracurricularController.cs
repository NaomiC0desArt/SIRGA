using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIRGA.Web.Helpers;
using SIRGA.Web.Models.ActividadExtracurricular;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Estudiante;
using SIRGA.Web.Models.Profesor;
using SIRGA.Web.Services;
using System.Net.Http;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActividadExtracurricularController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ImageUrlHelper _imageHelper;
        private readonly ILogger<ActividadExtracurricularController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;


        public ActividadExtracurricularController(
            ApiService apiService,
            IHttpClientFactory httpClientFactory,
            ImageUrlHelper imageHelper,
            ILogger<ActividadExtracurricularController> logger)
        {
            _apiService = apiService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _imageHelper = imageHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string searchTerm = "",
            string categoria = "",
            string sortBy = "nombre",
            int pageNumber = 1,
            int pageSize = 12)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<ActividadViewModel>>>(
                    "api/ActividadExtracurricular/Admin/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar las actividades";
                    return View(new PaginatedList<ActividadViewModel>(new List<ActividadViewModel>(), 0, 1, pageSize));
                }

                var actividades = response.Data ?? new List<ActividadViewModel>();

                foreach (var actividad in actividades)
                {
                    if (!string.IsNullOrEmpty(actividad.RutaImagen))
                    {
                        actividad.RutaImagen = _imageHelper.GetFullImageUrl(actividad.RutaImagen);
                    }
                }

                // Filtros
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    actividades = actividades.Where(a =>
                        (a.Nombre != null && a.Nombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (a.Descripcion != null && a.Descripcion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (a.NombreProfesor != null && a.NombreProfesor.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    actividades = actividades.Where(a => a.Categoria == categoria).ToList();
                }

                // Ordenamiento
                actividades = sortBy?.ToLower() switch
                {
                    "nombre" => actividades.OrderBy(a => a.Nombre).ToList(),
                    "categoria" => actividades.OrderBy(a => a.Categoria).ThenBy(a => a.Nombre).ToList(),
                    "inscritos" => actividades.OrderByDescending(a => a.TotalInscritos).ToList(),
                    "dia" => actividades.OrderBy(a => a.DiaSemana).ToList(),
                    _ => actividades.OrderBy(a => a.Nombre).ToList()
                };

                // Paginación
                var totalCount = actividades.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                var paginatedActividades = actividades
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginatedList = new PaginatedList<ActividadViewModel>(
                    paginatedActividades,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                ViewBag.SearchTerm = searchTerm;
                ViewBag.Categoria = categoria;
                ViewBag.SortBy = sortBy;
                ViewBag.PageSize = pageSize;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new PaginatedList<ActividadViewModel>(new List<ActividadViewModel>(), 0, 1, pageSize));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CreateActividadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();
                return View(model);
            }

            try
            {
                _logger.LogInformation("=== INICIANDO CREACIÓN DE ACTIVIDAD ===");
                _logger.LogInformation("Nombre: {Nombre}", model.Nombre);
                _logger.LogInformation("Categoría: {Categoria}", model.Categoria);
                _logger.LogInformation("Profesor ID: {IdProfesor}", model.IdProfesorEncargado);

                // Crear el contenido multipart
                using var content = new MultipartFormDataContent();

                // Agregar todos los campos
                content.Add(new StringContent(model.Nombre ?? string.Empty), "Nombre");
                content.Add(new StringContent(model.Descripcion ?? string.Empty), "Descripcion");
                content.Add(new StringContent(model.Categoria ?? string.Empty), "Categoria");
                content.Add(new StringContent(model.Requisitos ?? string.Empty), "Requisitos");

                // IMPORTANTE: Formato correcto para TimeSpan (formato c = constante)
                // O puedes usar el formato personalizado: @"hh\:mm\:ss"
                content.Add(new StringContent(model.HoraInicio.ToString("c")), "HoraInicio");
                content.Add(new StringContent(model.HoraFin.ToString("c")), "HoraFin");

                content.Add(new StringContent(model.DiaSemana ?? string.Empty), "DiaSemana");
                content.Add(new StringContent(model.Ubicacion ?? string.Empty), "Ubicacion");
                content.Add(new StringContent(model.ColorTarjeta ?? "#3B82F6"), "ColorTarjeta");
                content.Add(new StringContent(model.IdProfesorEncargado.ToString()), "IdProfesorEncargado");

                _logger.LogInformation("Campos agregados al FormData");

                // Agregar imagen si existe
                if (model.Imagen != null && model.Imagen.Length > 0)
                {
                    _logger.LogInformation("Procesando imagen: {FileName}, Tamaño: {Size} bytes",
                        model.Imagen.FileName, model.Imagen.Length);

                    var imageStream = new StreamContent(model.Imagen.OpenReadStream());
                    imageStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                        model.Imagen.ContentType ?? "image/jpeg");
                    content.Add(imageStream, "imagen", model.Imagen.FileName);
                }
                else
                {
                    _logger.LogInformation("No se proporcionó imagen");
                }

                // Crear cliente HTTP configurado
                var httpClient = _httpClientFactory.CreateClient("SIRGA_API");

                // Obtener y agregar token
                var token = HttpContext.Session.GetString("JWTToken");
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    _logger.LogInformation("✓ Token JWT agregado");
                }
                else
                {
                    _logger.LogError("✗ NO SE ENCONTRÓ TOKEN JWT - La petición probablemente fallará");
                    TempData["ErrorMessage"] = "Sesión expirada. Por favor, inicia sesión nuevamente.";
                    return RedirectToAction("Login", "Account");
                }

                var endpoint = "api/ActividadExtracurricular/Admin/Crear";
                _logger.LogInformation("→ Enviando POST a: {BaseAddress}{Endpoint}",
                    httpClient.BaseAddress, endpoint);

                var response = await httpClient.PostAsync(endpoint, content);

                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("← Respuesta recibida");
                _logger.LogInformation("Status Code: {StatusCode}", (int)response.StatusCode);
                _logger.LogInformation("Response Body: {Body}", responseBody);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✓✓✓ ACTIVIDAD CREADA EXITOSAMENTE ✓✓✓");
                    TempData["SuccessMessage"] = "Actividad creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("✗✗✗ ERROR AL CREAR ACTIVIDAD ✗✗✗");
                    _logger.LogError("Status: {Status}", response.StatusCode);
                    _logger.LogError("Detalle: {Detail}", responseBody);

                    // Intentar parsear el mensaje de error del API
                    string errorMessage = "Error al crear la actividad";
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<object>>(responseBody);
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                        {
                            errorMessage = errorResponse.Message;
                        }
                    }
                    catch { }

                    TempData["ErrorMessage"] = errorMessage;
                    await CargarDropdowns();
                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "!!! ERROR DE CONEXIÓN HTTP !!!");
                _logger.LogError("Mensaje: {Message}", ex.Message);
                _logger.LogError("Inner Exception: {Inner}", ex.InnerException?.Message);

                TempData["ErrorMessage"] = "Error de conexión. Verifica que el servidor API esté ejecutándose.";
                await CargarDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "!!! ERROR INESPERADO !!!");
                _logger.LogError("Tipo: {Type}", ex.GetType().Name);
                _logger.LogError("Mensaje: {Message}", ex.Message);
                _logger.LogError("StackTrace: {StackTrace}", ex.StackTrace);

                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                await CargarDropdowns();
                return View(model);
            }
            finally
            {
                _logger.LogInformation("=== FIN DEL PROCESO DE CREACIÓN ===");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<ActividadViewModel>>(
                $"api/ActividadExtracurricular/Admin/{id}");

            if (response?.Success != true)
            {
                TempData["ErrorMessage"] = "Actividad no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var updateModel = new UpdateActividadViewModel
            {
                Nombre = response.Data.Nombre,
                Descripcion = response.Data.Descripcion,
                Categoria = response.Data.Categoria,
                Requisitos = response.Data.Requisitos,
                HoraInicio = response.Data.HoraInicio,
                HoraFin = response.Data.HoraFin,
                DiaSemana = response.Data.DiaSemana,
                Ubicacion = response.Data.Ubicacion,
                ColorTarjeta = response.Data.ColorTarjeta,
                IdProfesorEncargado = response.Data.IdProfesorEncargado,
                EstaActiva = response.Data.EstaActiva,
                RutaImagenActual = response.Data.RutaImagen
            };

            await CargarDropdowns();
            ViewBag.ActividadId = id;
            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, UpdateActividadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdowns();
                ViewBag.ActividadId = id;
                return View(model);
            }

            try
            {
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(model.Nombre), "Nombre");
                content.Add(new StringContent(model.Descripcion), "Descripcion");
                content.Add(new StringContent(model.Categoria), "Categoria");
                content.Add(new StringContent(model.Requisitos ?? ""), "Requisitos");
                content.Add(new StringContent(model.HoraInicio.ToString()), "HoraInicio");
                content.Add(new StringContent(model.HoraFin.ToString()), "HoraFin");
                content.Add(new StringContent(model.DiaSemana), "DiaSemana");
                content.Add(new StringContent(model.Ubicacion ?? ""), "Ubicacion");
                content.Add(new StringContent(model.ColorTarjeta), "ColorTarjeta");
                content.Add(new StringContent(model.IdProfesorEncargado.ToString()), "IdProfesorEncargado");
                content.Add(new StringContent(model.EstaActiva.ToString()), "EstaActiva");

                if (model.Imagen != null)
                {
                    var fileContent = new StreamContent(model.Imagen.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Imagen.ContentType);
                    content.Add(fileContent, "imagen", model.Imagen.FileName);
                }

                var client = new HttpClient { BaseAddress = new Uri("https://localhost:7166/") };
                var response = await client.PutAsync($"api/ActividadExtracurricular/Admin/Actualizar/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Actividad actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error al actualizar la actividad";
                await CargarDropdowns();
                ViewBag.ActividadId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar actividad");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                await CargarDropdowns();
                ViewBag.ActividadId = id;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<ActividadDetalleViewModel>>(
                $"api/ActividadExtracurricular/Admin/{id}");

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

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/ActividadExtracurricular/Admin/Eliminar/{id}");

                if (response)
                {
                    TempData["SuccessMessage"] = "Actividad eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la actividad";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar actividad");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GestionarInscritos(int id)
        {
            var actividadResponse = await _apiService.GetAsync<ApiResponse<ActividadDetalleViewModel>>(
                $"api/ActividadExtracurricular/Admin/{id}");

            if (actividadResponse?.Success != true)
            {
                TempData["ErrorMessage"] = "Actividad no encontrada";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(actividadResponse.Data.RutaImagen))
            {
                actividadResponse.Data.RutaImagen = _imageHelper.GetFullImageUrl(actividadResponse.Data.RutaImagen);
            }

            // Obtener todos los estudiantes para el dropdown
            var estudiantesResponse = await _apiService.GetAsync<ApiResponse<List<EstudianteDto>>>(
                "api/Estudiante/GetAll");

            ViewBag.Actividad = actividadResponse.Data;
            ViewBag.EstudiantesDisponibles = estudiantesResponse?.Data?
                .Where(e => e.IsActive && !actividadResponse.Data.EstudiantesInscritos.Any(ei => ei.IdEstudiante == e.Id))
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.FirstName} {e.LastName} - {e.Matricula}"
                })
                .ToList() ?? new List<SelectListItem>();

            return View(actividadResponse.Data);
        }

        [HttpPost]
        public async Task<IActionResult> InscribirEstudiante(int idActividad, int idEstudiante)
        {
            try
            {
                var response = await _apiService.PostAsync<object, ApiResponse<bool>>(
                    $"api/ActividadExtracurricular/Admin/{idActividad}/Inscribir/{idEstudiante}",
                    new { });

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Estudiante inscrito exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al inscribir estudiante";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inscribir estudiante");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(GestionarInscritos), new { id = idActividad });
        }

        [HttpPost]
        public async Task<IActionResult> DesinscribirEstudiante(int idActividad, int idEstudiante)
        {
            try
            {
                var response = await _apiService.PostAsync<object, ApiResponse<bool>>(
                    $"api/ActividadExtracurricular/Admin/{idActividad}/Desinscribir/{idEstudiante}",
                    new { });

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Estudiante desinscrito exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al desinscribir estudiante";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desinscribir estudiante");
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }

            return RedirectToAction(nameof(GestionarInscritos), new { id = idActividad });
        }

        private async Task CargarDropdowns()
        {
            try
            {
                // Profesores
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
                }
                else
                {
                    ViewBag.Profesores = new SelectList(Enumerable.Empty<SelectListItem>());
                }

                // Categorías
                var categorias = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Curso", Text = "Curso" },
                    new SelectListItem { Value = "Actividad/Voluntariado", Text = "Actividad/Voluntariado" },
                    new SelectListItem { Value = "Club", Text = "Club" }
                };
                ViewBag.Categorias = new SelectList(categorias, "Value", "Text");

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
                ViewBag.Profesores = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.Categorias = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.DiasSemana = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}
