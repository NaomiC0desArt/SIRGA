using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System.Calificacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;
        public CalificacionController(ICalificacionService service)
        {
            _calificacionService = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _calificacionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetCalificacionById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _calificacionService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] CalificacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _calificacionService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetCalificacionById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CalificacionDto dto)
        {
            var result = await _calificacionService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("Publicar/{id:int}")]
        public async Task<IActionResult> Publish(int id)
        {
            var result = await _calificacionService.PublicarCalificacionAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _calificacionService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("EstudianteId: {estudianteId:int}, AsignaturaId: {asignaturaId:int}, CursoId: {cursoId:int}, AnioEscolarId: {anioEscolarId:int}")]
        public async Task<IActionResult> GetAnnualGradesAsync(int estudianteId, int asignaturaId, int cursoId, int anioEscolarId)
        {
            var result = await _calificacionService.GetAnnualGradesAsync(estudianteId, asignaturaId, cursoId, anioEscolarId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
