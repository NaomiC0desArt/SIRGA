using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaseProgramadaController : ControllerBase
    {
        private readonly IClaseProgramadaService _claseProgramadaService;

        public ClaseProgramadaController(IClaseProgramadaService claseProgramadaService)
        {
            _claseProgramadaService = claseProgramadaService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _claseProgramadaService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetClaseProgramadaById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _claseProgramadaService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] ClaseProgramadaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _claseProgramadaService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetClaseProgramadaById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClaseProgramadaDto dto)
        {
            var result = await _claseProgramadaService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _claseProgramadaService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
