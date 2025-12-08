using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System.Calificacion
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AnioEscolarController : ControllerBase
    {
        private readonly IAnioEscolarService _anioEscolarService;
        public AnioEscolarController(IAnioEscolarService service)
        {
            _anioEscolarService = service;
        }
        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _anioEscolarService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetAnioEscolarById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _anioEscolarService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] AnioEscolarDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _anioEscolarService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetAnioEscolarById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnioEscolarDto dto)
        {
            var result = await _anioEscolarService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _anioEscolarService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
