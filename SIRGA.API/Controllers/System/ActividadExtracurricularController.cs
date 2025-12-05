using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Entities.ActividadExtracurricular;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Interfaces;
using System.Security.Claims;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadExtracurricularController : ControllerBase
    {
        private readonly IActividadExtracurricularService _actividadService;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly ILogger<ActividadExtracurricularController> _logger;

        public ActividadExtracurricularController(
            IActividadExtracurricularService actividadService,
            IEstudianteRepository estudianteRepository,
            ILogger<ActividadExtracurricularController> logger)
        {
            _actividadService = actividadService;
            _estudianteRepository = estudianteRepository;
            _logger = logger;
        }

        // ============ ENDPOINTS ADMIN ============

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin/GetAll")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var result = await _actividadService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin/{id:int}")]
        public async Task<IActionResult> GetByIdAdmin(int id)
        {
            var result = await _actividadService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/Crear")]
        public async Task<IActionResult> Create([FromForm] CreateActividadDto dto, IFormFile imagen = null)
        {
            _logger.LogInformation("=== REQUEST RECIBIDO EN API ===");
            _logger.LogInformation("Datos del formulario recibidos");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("ModelState inválido: {Errors}", string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            _logger.LogInformation("ModelState válido, llamando al servicio...");
            var result = await _actividadService.CreateAsync(dto, imagen);

            if (!result.Success)
            {
                _logger.LogWarning("Servicio retornó error: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("✓ Actividad creada exitosamente");

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Admin/Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateActividadDto dto, IFormFile imagen = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _actividadService.UpdateAsync(id, dto, imagen);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Admin/Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _actividadService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ============ GESTIÓN DE INSCRIPCIONES (ADMIN) ============

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin/{idActividad:int}/Estudiantes")]
        public async Task<IActionResult> GetEstudiantesInscritos(int idActividad)
        {
            var result = await _actividadService.GetEstudiantesInscritosAsync(idActividad);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/{idActividad:int}/Inscribir/{idEstudiante:int}")]
        public async Task<IActionResult> InscribirEstudiante(int idActividad, int idEstudiante)
        {
            var result = await _actividadService.InscribirEstudianteAsync(idActividad, idEstudiante);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/{idActividad:int}/Desinscribir/{idEstudiante:int}")]
        public async Task<IActionResult> DesinscribirEstudiante(int idActividad, int idEstudiante)
        {
            var result = await _actividadService.DesinscribirEstudianteAsync(idActividad, idEstudiante);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // ============ ENDPOINTS ESTUDIANTE ============

        [Authorize(Roles = "Estudiante")]
        [HttpGet("Estudiante/Disponibles")]
        public async Task<IActionResult> GetActividadesDisponibles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.GetActividadesActivasAsync(estudiante.Id);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpGet("Estudiante/Por-Categoria/{categoria}")]
        public async Task<IActionResult> GetPorCategoria(string categoria)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.GetPorCategoriaAsync(categoria, estudiante.Id);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpGet("Estudiante/{id:int}/Detalle")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.GetByIdAsync(id, estudiante.Id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost("Estudiante/{id:int}/Inscribirse")]
        public async Task<IActionResult> Inscribirse(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.InscribirseAsync(id, estudiante.Id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost("Estudiante/{id:int}/Desinscribirse")]
        public async Task<IActionResult> Desinscribirse(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.DesinscribirseAsync(id, estudiante.Id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
