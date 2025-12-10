using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Web.Models.AnioEscolar;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Aula;
using SIRGA.Web.Models.CursoAcademico;
using SIRGA.Web.Models.Grado;
using SIRGA.Web.Models.Seccion;
using SIRGA.Web.Services;
using CreateCursoAcademicoDto = SIRGA.Web.Models.CursoAcademico.CreateCursoAcademicoDto;

namespace SIRGA.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CursoAcademicoController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CursoAcademicoController> _logger;

        public CursoAcademicoController(ApiService apiService, ILogger<CursoAcademicoController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // ==================== VISTA PRINCIPAL (con modales) ====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<CursoAcademicoDto>>>("api/CursoAcademico/GetAll");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = response?.Message ?? "Error al cargar los cursos académicos";
                    return View(new List<CursoAcademicoDto>());
                }

                return View(response.Data ?? new List<CursoAcademicoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cursos académicos");
                TempData["ErrorMessage"] = "Error de conexión con el servidor";
                return View(new List<CursoAcademicoDto>());
            }
        }

        // ==================== DETALLES ====================
        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CursoAcademicoDto>>($"api/CursoAcademico/{id}");

                if (response?.Success != true)
                {
                    TempData["ErrorMessage"] = "Curso académico no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del curso {Id}", id);
                TempData["ErrorMessage"] = "Error al cargar los detalles";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==================== API ENDPOINTS (para modales y AJAX) ====================

        /// <summary>
        /// Obtiene los datos de un curso para el modal de edición
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerCurso(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CursoAcademicoDto>>($"api/CursoAcademico/{id}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Curso no encontrado" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener curso {Id}", id);
                return Json(new { success = false, message = "Error al cargar el curso" });
            }
        }

        /// <summary>
        /// Obtiene las secciones disponibles para un grado y año escolar
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SeccionesDisponibles(int idGrado, int idAnioEscolar)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<SeccionDto>>>(
                    $"api/CursoAcademico/SeccionesDisponibles?idGrado={idGrado}&idAnioEscolar={idAnioEscolar}");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Error al cargar secciones" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener secciones disponibles");
                return Json(new { success = false, message = "Error al cargar secciones disponibles" });
            }
        }

        /// <summary>
        /// Crea un nuevo curso académico (llamado desde modal)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CreateCursoAcademicoDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Datos inválidos", errors });
            }

            try
            {
                var response = await _apiService.PostAsync<CreateCursoAcademicoDto, ApiResponse<CursoAcademicoDto>>(
                    "api/CursoAcademico/Crear", model);

                if (response?.Success == true)
                {
                    return Json(new { success = true, message = "✅ Curso académico creado exitosamente" });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al crear el curso académico",
                    errors = response?.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear curso académico");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Actualiza un curso académico (llamado desde modal)
        /// </summary>
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CreateCursoAcademicoDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Datos inválidos", errors });
            }

            try
            {
                var response = await _apiService.PutAsync($"api/CursoAcademico/Actualizar/{id}", model);

                if (response)
                {
                    return Json(new { success = true, message = "✅ Curso académico actualizado exitosamente" });
                }

                return Json(new { success = false, message = "Error al actualizar el curso académico" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar curso académico");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Elimina un curso académico
        /// </summary>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/CursoAcademico/Eliminar/{id}");

                if (response)
                {
                    return Json(new { success = true, message = "🗑️ Curso académico eliminado exitosamente" });
                }

                return Json(new { success = false, message = "❌ Error al eliminar el curso académico" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar curso académico {Id}", id);
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSeccionAutomatica()
        {
            try
            {
                var response = await _apiService.PostAsync<object, ApiResponse<SeccionDto>>(
                    "api/Seccion/CrearAutomatica",
                    new { });

                if (response?.Success == true)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Sección {response.Data.Nombre} creada exitosamente",
                        data = response.Data
                    });
                }

                return Json(new
                {
                    success = false,
                    message = response?.Message ?? "Error al crear la sección"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear sección automática");
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }

        #region helpers servir datos
        [HttpGet]
        public async Task<IActionResult> ObtenerGrados()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<GradoDto>>>("api/Grado/GetAll");
                return Json(new { success = response?.Success == true, data = response?.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar grados");
                return Json(new { success = false, message = "Error al cargar grados" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAniosEscolares()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AnioEscolarDto>>>("api/AnioEscolar/GetAll");
                return Json(new { success = response?.Success == true, data = response?.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar años escolares");
                return Json(new { success = false, message = "Error al cargar años escolares" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAulasDisponibles()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<AulaDto>>>("api/Aula/Disponibles");
                return Json(new { success = response?.Success == true, data = response?.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar aulas");
                return Json(new { success = false, message = "Error al cargar aulas" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSecciones()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<SeccionDto>>>("api/Seccion/GetAll");

                if (response?.Success != true)
                {
                    return Json(new { success = false, message = "Error al cargar secciones" });
                }

                return Json(new { success = true, data = response.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar secciones");
                return Json(new { success = false, message = "Error al cargar secciones" });
            }
        }
        #endregion
    }
}
