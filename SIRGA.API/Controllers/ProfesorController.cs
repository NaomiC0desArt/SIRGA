using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.UserManagement.Estudiante;
using SIRGA.Application.DTOs.UserManagement.Profesor;
using SIRGA.Application.Interfaces.Usuarios;
using System.Security.Claims;

namespace SIRGA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesorController : ControllerBase
    {
        private readonly IProfesorService _profesorService;

        public ProfesorController(IProfesorService profesorService)
        {
            _profesorService = profesorService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _profesorService.GetAllProfesoresAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}", Name = "GetProfesorById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _profesorService.GetProfesorByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] CreateProfesorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _profesorService.CreateProfesorAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetProfesorById", new { id = result.Data.Id }, result);
        }

        [Authorize(Roles = "Profesor")]
        [HttpPost("Completar-Perfil")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteTeacherProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener el ID del usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _profesorService.CompleteProfileAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProfesorDto dto)
        {
            var result = await _profesorService.UpdateProfesorAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _profesorService.DeleteProfesorAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("Activar/{id:int}")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _profesorService.ActivateAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("Desactivar/{id:int}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _profesorService.DeactivateAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
