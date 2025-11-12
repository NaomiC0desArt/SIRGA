using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionController : ControllerBase
    {
        private readonly IInscripcionService _inscripcionService;

        public InscripcionController(IInscripcionService inscripcionService)
        {
            _inscripcionService = inscripcionService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _inscripcionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetInscripcionById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _inscripcionService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] InscripcionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _inscripcionService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetInscripcionById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InscripcionDto dto)
        {
            var result = await _inscripcionService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _inscripcionService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
