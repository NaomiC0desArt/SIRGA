using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.UserManagement.Estudiante;
using SIRGA.Application.Interfaces.Usuarios;
using System.Security.Claims;

namespace SIRGA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {
        private readonly IEstudianteService _estudianteService;
        private readonly ILogger<EstudianteController> _logger; // ✅ Agregar logger

        public EstudianteController(
            IEstudianteService estudianteService,
            ILogger<EstudianteController> logger) // ✅ Inyectar logger
        {
            _estudianteService = estudianteService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _estudianteService.GetAllEstudiantesAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetEstudianteById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _estudianteService.GetEstudianteByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] CreateEstudianteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _estudianteService.CreateEstudianteAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetEstudianteById", new { id = result.Data.Id }, result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost("Completar-Perfil")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteStudentProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener el ID del usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _estudianteService.CompleteProfileAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEstudianteDto dto)
        {
            var result = await _estudianteService.UpdateEstudianteAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estudianteService.DeleteEstudianteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("Activar/{id:int}")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _estudianteService.ActivateAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("Desactivar/{id:int}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _estudianteService.DeactivateAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // ✅ MEJORADO: Obtener ID del estudiante con logs
        [Authorize(Roles = "Estudiante")]
        [HttpGet("Mi-Id")]
        public async Task<IActionResult> GetMyEstudianteId()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation($"🔍 [API] Obteniendo ID de estudiante para userId: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("⚠️ Usuario no autenticado");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                var result = await _estudianteService.GetEstudianteIdByUserIdAsync(userId);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Estudiante no encontrado para userId: {userId}");
                    return NotFound(result);
                }

                _logger.LogInformation($"✅ Estudiante ID encontrado: {result.Data}");
                return Ok(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener ID del estudiante");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }
    }
}
