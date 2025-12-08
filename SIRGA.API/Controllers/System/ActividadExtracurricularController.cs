using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Entities.ActividadExtracurricular;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Interfaces.Usuarios;
using SIRGA.Domain.Interfaces;
using System.Security.Claims;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadExtracurricularController : ControllerBase
    {
        private readonly IActividadExtracurricularService _actividadService;
        private readonly IEstudianteService _estudianteService; 
        private readonly ILogger<ActividadExtracurricularController> _logger;

        public ActividadExtracurricularController(
            IActividadExtracurricularService actividadService,
            IEstudianteService estudianteService, 
            ILogger<ActividadExtracurricularController> logger)
        {
            _actividadService = actividadService;
            _estudianteService = estudianteService;
            _logger = logger;
        }

        #region Admin Endpoints
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("ModelState inválido: {Errors}", string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            var result = await _actividadService.CreateAsync(dto, imagen);

            if (!result.Success)
            {
                _logger.LogWarning("Error al crear actividad: {Message}", result.Message);
                return BadRequest(result);
            }

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
        #endregion

        #region Estudiante Endpoints
        [Authorize(Roles = "Estudiante")]
        [HttpGet("Estudiante/Disponibles")]
        public async Task<IActionResult> GetActividadesDisponibles()
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.GetActividadesActivasAsync(estudianteId.Value);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpGet("Estudiante/Por-Categoria/{categoria}")]
        public async Task<IActionResult> GetPorCategoria(string categoria)
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.GetPorCategoriaAsync(categoria, estudianteId.Value);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpGet("Estudiante/{id:int}/Detalle")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.GetByIdAsync(id, estudianteId.Value);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost("Estudiante/{id:int}/Inscribirse")]
        public async Task<IActionResult> Inscribirse(int id)
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.InscribirseAsync(id, estudianteId.Value);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost("Estudiante/{id:int}/Desinscribirse")]
        public async Task<IActionResult> Desinscribirse(int id)
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _actividadService.DesinscribirseAsync(id, estudianteId.Value);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        #endregion

        #region Helper Methods
        private async Task<int?> GetEstudianteIdFromUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return null;

            var result = await _estudianteService.GetEstudianteIdByUserIdAsync(userId);

            return result.Success ? result.Data : null;
        }
        #endregion
    }
}
