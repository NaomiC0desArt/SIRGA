using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities.Calificacion;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Calificacion;
using SIRGA.Web.Services;
using System.Security.Claims;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AsignaturaProfesorDto = SIRGA.Web.Models.Calificacion.AsignaturaProfesorDto;
using CalificacionEstudianteViewDto = SIRGA.Web.Models.Calificacion.CalificacionEstudianteViewDto;
using CapturaMasivaDto = SIRGA.Web.Models.Calificacion.CapturaMasivaDto;
using EditarCalificacionDto = SIRGA.Web.Models.Calificacion.EditarCalificacionDto;
using EstudianteBusquedaDto = SIRGA.Web.Models.Calificacion.EstudianteBusquedaDto;
using GuardarCalificacionesRequestDto = SIRGA.Web.Models.Calificacion.GuardarCalificacionesRequestDto;
using HistorialCalificacionDto = SIRGA.Web.Models.Calificacion.HistorialCalificacionDto;
using PublicarCalificacionesDto = SIRGA.Web.Models.Calificacion.PublicarCalificacionesDto;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Profesor,Admin")]
        public class CalificacionController : Controller
        {
            private readonly ApiService _apiService;
            private readonly ILogger<CalificacionController> _logger;

            public CalificacionController(ApiService apiService, ILogger<CalificacionController> logger)
            {
                _apiService = apiService;
                _logger = logger;
            }

            // ==================== PROFESOR ====================

            [Authorize(Roles = "Profesor")]
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                try
                {
                    _logger.LogInformation("🏠 Cargando pantalla de asignaturas...");

                    var response = await _apiService.GetAsync<ApiResponse<List<AsignaturaProfesorDto>>>(
                        "api/Calificacion/Mis-Asignaturas");

                    if (response?.Success != true)
                    {
                        _logger.LogWarning($"⚠️ Error en respuesta: {response?.Message}");
                        TempData["ErrorMessage"] = response?.Message ?? "Error al cargar asignaturas";
                        return View(new List<AsignaturaProfesorDto>());
                    }

                    _logger.LogInformation($"✅ {response.Data?.Count ?? 0} asignaturas cargadas");
                    return View(response.Data ?? new List<AsignaturaProfesorDto>());
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("⚠️ Token expirado - redirigiendo a login");
                    TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar asignaturas del profesor");
                    TempData["ErrorMessage"] = "Error de conexión con el servidor";
                    return View(new List<AsignaturaProfesorDto>());
                }
            }

            [Authorize(Roles = "Profesor")]
            [HttpGet]
            public async Task<IActionResult> Calificar(int idAsignatura, int idCurso)
            {
                try
                {
                    _logger.LogInformation($"📝 Cargando pantalla de calificación - Asignatura: {idAsignatura}, Curso: {idCurso}");

                    var response = await _apiService.GetAsync<ApiResponse<CapturaMasivaDto>>(
                        $"api/Calificacion/Estudiantes-Para-Calificar?idAsignatura={idAsignatura}&idCursoAcademico={idCurso}");

                    if (response?.Success != true)
                    {
                        _logger.LogWarning($"⚠️ Error en respuesta: {response?.Message}");
                        TempData["ErrorMessage"] = response?.Message ?? "Error al cargar estudiantes";
                        return RedirectToAction(nameof(Index));
                    }

                    _logger.LogInformation($"✅ {response.Data?.Calificaciones?.Count ?? 0} estudiantes cargados");

                    ViewBag.AsignaturaNombre = "Asignatura";
                    ViewBag.TipoAsignatura = response.Data.TipoAsignatura ?? "";
                    ViewBag.CursoNombre = "Curso";
                    ViewBag.NumeroPeriodo = 1;

                    try
                    {
                        var asignaturaResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>($"api/Asignatura/{idAsignatura}");
                        if (asignaturaResponse?.Success == true)
                        {
                            ViewBag.AsignaturaNombre = asignaturaResponse.Data.GetProperty("nombre").GetString();
                        }

                        var cursoResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>($"api/CursoAcademico/{idCurso}");
                        if (cursoResponse?.Success == true)
                        {
                            var grado = cursoResponse.Data.GetProperty("grado").GetProperty("gradeName").GetString();
                            var seccion = cursoResponse.Data.GetProperty("seccion").GetProperty("nombre").GetString();
                            ViewBag.CursoNombre = $"{grado} - Sección {seccion}";
                        }

                        var periodoResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>("api/Periodo/Activo");
                        if (periodoResponse?.Success == true)
                        {
                            ViewBag.NumeroPeriodo = periodoResponse.Data.GetProperty("numero").GetInt32();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ No se pudieron cargar datos adicionales");
                    }

                    return View(response.Data);
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("⚠️ Token expirado - redirigiendo a login");
                    TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar pantalla de calificación");
                    TempData["ErrorMessage"] = "Error al cargar datos";
                    return RedirectToAction(nameof(Index));
                }
            }

            [Authorize(Roles = "Profesor")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Guardar([FromBody] GuardarCalificacionesRequestDto model)
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("⚠️ ModelState inválido");
                    return Json(new { success = false, message = "Datos inválidos" });
                }

                try
                {
                    _logger.LogInformation($"💾 Guardando {model.Calificaciones?.Count ?? 0} calificaciones");

                    var response = await _apiService.PostAsync<GuardarCalificacionesRequestDto, ApiResponse<bool>>(
                        "api/Calificacion/Guardar", model);

                    if (response?.Success == true)
                    {
                        _logger.LogInformation("✅ Calificaciones guardadas exitosamente");
                        return Json(new { success = true, message = "Calificaciones guardadas exitosamente" });
                    }

                    _logger.LogWarning($"⚠️ Error al guardar: {response?.Message}");
                    return Json(new { success = false, message = response?.Message ?? "Error al guardar calificaciones" });
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("⚠️ Token expirado durante guardado");
                    return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página e inicia sesión nuevamente." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al guardar calificaciones");
                    return Json(new { success = false, message = "Error al procesar la solicitud" });
                }
            }

            [Authorize(Roles = "Profesor")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Publicar([FromBody] PublicarCalificacionesDto model)
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("⚠️ ModelState inválido", string.Join(", ", errors));
                    return Json(new { success = false, message = "Datos inválidos" });
                }

                try
                {
                    _logger.LogInformation($"📤 Publicando calificaciones - Asignatura: {model.IdAsignatura}");

                    var response = await _apiService.PostAsync<PublicarCalificacionesDto, ApiResponse<bool>>(
                        "api/Calificacion/Publicar", model);

                    if (response?.Success == true)
                    {
                        _logger.LogInformation("✅ Calificaciones publicadas exitosamente");
                        return Json(new { success = true, message = response.Message ?? "Calificaciones publicadas exitosamente" });
                    }

                    _logger.LogWarning($"⚠️ Error al publicar: {response?.Message}");
                    return Json(new { success = false, message = response?.Message ?? "Error al publicar calificaciones" });
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("⚠️ Token expirado durante publicación");
                    return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página e inicia sesión nuevamente." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al publicar calificaciones");
                    return Json(new { success = false, message = "Error al procesar la solicitud" });
                }
            }

            // ==================== ADMIN Y PROFESOR ====================

            [Authorize(Roles = "Admin,Profesor")]
            [HttpGet]
            public async Task<IActionResult> BuscarCalificaciones()
            {
                try
                {
                    _logger.LogInformation("🔍 Cargando pantalla de búsqueda de calificaciones");
                    return View(new List<EstudianteBusquedaDto>());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar búsqueda");
                    TempData["ErrorMessage"] = "Error al cargar la página";
                    return RedirectToAction("Index", User.IsInRole("Admin") ? "Admin" : "Profesor");
                }
            }

            [Authorize(Roles = "Admin,Profesor")]
            [HttpGet]
            public async Task<IActionResult> BuscarEstudiantes(string searchTerm = "", int? idGrado = null, int? idCursoAcademico = null)
            {
                try
                {
                    _logger.LogInformation($"🔍 Buscando estudiantes: '{searchTerm}'");

                    var query = $"api/Calificacion/Buscar-Estudiantes?searchTerm={searchTerm}";
                    if (idGrado.HasValue) query += $"&idGrado={idGrado}";
                    if (idCursoAcademico.HasValue) query += $"&idCursoAcademico={idCursoAcademico}";

                    var response = await _apiService.GetAsync<ApiResponse<List<EstudianteBusquedaDto>>>(query);

                    if (response?.Success != true)
                    {
                        _logger.LogWarning($"⚠️ Error: {response?.Message}");
                        return Json(new { success = false, message = response?.Message ?? "Error al buscar estudiantes" });
                    }

                    _logger.LogInformation($"✅ {response.Data?.Count ?? 0} estudiantes encontrados");
                    return Json(new { success = true, data = response.Data ?? new List<EstudianteBusquedaDto>() });
                }
                catch (UnauthorizedAccessException)
                {
                    return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al buscar estudiantes");
                    return Json(new { success = false, message = "Error al buscar estudiantes" });
                }
            }

            [Authorize(Roles = "Admin,Profesor")]
            [HttpGet]
            public async Task<IActionResult> VerCalificacionesEstudiante(int estudianteId)
            {
                try
                {
                    _logger.LogInformation($"📊 Ver calificaciones del estudiante {estudianteId}");

                    var response = await _apiService.GetAsync<ApiResponse<List<CalificacionEstudianteViewDto>>>(
                        $"api/Calificacion/Estudiante/{estudianteId}");

                    if (response?.Success != true)
                    {
                        _logger.LogWarning($"⚠️ Error: {response?.Message}");
                        TempData["ErrorMessage"] = response?.Message ?? "Error al cargar calificaciones";
                        return RedirectToAction(nameof(BuscarCalificaciones));
                    }

                    try
                    {
                        var estudianteResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>($"api/Estudiante/{estudianteId}");

                        if (estudianteResponse?.Success == true)
                        {
                            var estudianteData = estudianteResponse.Data;
                            ViewBag.EstudianteId = estudianteId;
                            ViewBag.EstudianteNombre = $"{estudianteData.GetProperty("firstName").GetString()} {estudianteData.GetProperty("lastName").GetString()}";
                            ViewBag.EstudianteMatricula = estudianteData.GetProperty("matricula").GetString();
                            ViewBag.CursoNombre = "Curso Actual";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ No se pudieron cargar datos del estudiante");
                        ViewBag.EstudianteId = estudianteId;
                        ViewBag.EstudianteNombre = "Estudiante";
                        ViewBag.EstudianteMatricula = "N/A";
                        ViewBag.CursoNombre = "N/A";
                    }

                    _logger.LogInformation($"✅ Calificaciones cargadas correctamente");
                    return View(response.Data ?? new List<CalificacionEstudianteViewDto>());
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("⚠️ Token expirado");
                    TempData["ErrorMessage"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar calificaciones del estudiante");
                    TempData["ErrorMessage"] = "Error de conexión con el servidor";
                    return RedirectToAction(nameof(BuscarCalificaciones));
                }
            }

            // ==================== SOLO ADMIN ====================

            [Authorize(Roles = "Admin")]
            [HttpGet]
            public async Task<IActionResult> EditarCalificacion(int estudianteId, int asignaturaId, int periodo)
            {
                try
                {
                    _logger.LogInformation($"✏️ Editar calificación - Estudiante: {estudianteId}, Asignatura: {asignaturaId}, Período: {periodo}");

                    var response = await _apiService.GetAsync<ApiResponse<List<CalificacionEstudianteViewDto>>>(
                        $"api/Calificacion/Estudiante/{estudianteId}");

                    if (response?.Success != true || response.Data == null)
                    {
                        TempData["ErrorMessage"] = "No se pudo cargar la calificación";
                        return RedirectToAction(nameof(BuscarCalificaciones));
                    }

                    var asignatura = response.Data.FirstOrDefault(a => a.IdAsignatura == asignaturaId);
                    if (asignatura == null)
                    {
                        TempData["ErrorMessage"] = "Asignatura no encontrada";
                        return RedirectToAction(nameof(BuscarCalificaciones));
                    }

                    var periodoData = asignatura.Periodos.FirstOrDefault(p => p.NumeroPeriodo == periodo);
                    if (periodoData == null || !periodoData.Publicada)
                    {
                        TempData["ErrorMessage"] = "La calificación no está publicada o no existe";
                        return RedirectToAction(nameof(BuscarCalificaciones));
                    }

                    var model = new EditarCalificacionViewModel
                    {
                        IdEstudiante = estudianteId,
                        IdAsignatura = asignaturaId,
                        IdPeriodo = periodo,
                        AsignaturaNombre = asignatura.AsignaturaNombre,
                        Componentes = periodoData.Componentes,
                        TotalActual = periodoData.Total ?? 0
                    };

                    try
                    {
                        var estudianteResponse = await _apiService.GetAsync<ApiResponse<JsonElement>>($"api/Estudiante/{estudianteId}");
                        if (estudianteResponse?.Success == true)
                        {
                            var estudianteData = estudianteResponse.Data;
                            ViewBag.EstudianteNombre = $"{estudianteData.GetProperty("firstName").GetString()} {estudianteData.GetProperty("lastName").GetString()}";
                            ViewBag.EstudianteMatricula = estudianteData.GetProperty("matricula").GetString();
                        }
                    }
                    catch
                    {
                        ViewBag.EstudianteNombre = "Estudiante";
                        ViewBag.EstudianteMatricula = "N/A";
                    }

                    ViewBag.NumeroPeriodo = periodo;

                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar pantalla de edición");
                    TempData["ErrorMessage"] = "Error al cargar la calificación";
                    return RedirectToAction(nameof(BuscarCalificaciones));
                }
            }

            [Authorize(Roles = "Admin")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditarCalificacion([FromBody] EditarCalificacionDto model)
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Datos inválidos" });
                }

                try
                {
                    _logger.LogInformation($"💾 Guardando edición de calificación");

                    var response = await _apiService.PutAsync("api/Calificacion/Editar", model);

                    if (response)
                    {
                        _logger.LogInformation("✅ Calificación editada exitosamente");
                        return Json(new { success = true, message = "Calificación actualizada exitosamente" });
                    }

                    _logger.LogWarning("⚠️ Error al editar calificación");
                    return Json(new { success = false, message = "Error al actualizar la calificación" });
                }
                catch (UnauthorizedAccessException)
                {
                    return Json(new { success = false, message = "Tu sesión ha expirado. Recarga la página." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al editar calificación");
                    return Json(new { success = false, message = "Error al procesar la solicitud" });
                }
            }

            [Authorize(Roles = "Admin")]
            [HttpGet]
            public async Task<IActionResult> HistorialCalificacion(int calificacionId)
            {
                try
                {
                    _logger.LogInformation($"📜 Historial de calificación {calificacionId}");

                    var response = await _apiService.GetAsync<ApiResponse<List<HistorialCalificacionDto>>>(
                        $"api/Calificacion/Historial/{calificacionId}");

                    if (response?.Success != true)
                    {
                        _logger.LogWarning($"⚠️ Error: {response?.Message}");
                        TempData["ErrorMessage"] = response?.Message ?? "Error al cargar historial";
                        return RedirectToAction(nameof(BuscarCalificaciones));
                    }

                    _logger.LogInformation($"✅ {response.Data?.Count ?? 0} registros de historial");
                    return View(response.Data ?? new List<HistorialCalificacionDto>());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al cargar historial");
                    TempData["ErrorMessage"] = "Error al cargar historial";
                    return RedirectToAction(nameof(BuscarCalificaciones));
                }
            }
        }
    }

