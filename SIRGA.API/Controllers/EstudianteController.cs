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

        public EstudianteController(IEstudianteService estudianteService)
        {
            _estudianteService = estudianteService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _estudianteService.GetAllEstudiantesAsync();
            return Ok(result);
        }


        [HttpGet("{id:int}", Name ="GetEstudianteById")]
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
        [Authorize(Roles = "Estudiante")]
        [HttpGet("Mi-Id")]
        public async Task<IActionResult> GetMyEstudianteId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _estudianteService.GetEstudianteIdByUserIdAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(new { id = result.Data });
        }
    }
}
