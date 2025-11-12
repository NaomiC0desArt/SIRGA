using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradoController : ControllerBase
    {
        private readonly IGradoService _gradoService;

        public GradoController(IGradoService gradoService)
        {
            _gradoService = gradoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _gradoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetGradoById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _gradoService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] GradoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _gradoService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetGradoById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GradoDto dto)
        {
            var result = await _gradoService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _gradoService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
