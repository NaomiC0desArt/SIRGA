using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AsignaturaController : ControllerBase
    {
        private readonly IAsignaturaService _asignaturaService;

        public AsignaturaController(IAsignaturaService service)
        {
            _asignaturaService = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _asignaturaService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetAsignaturaById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _asignaturaService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] AsignaturaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _asignaturaService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetAsignaturaById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AsignaturaDto dto)
        {
            var result = await _asignaturaService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _asignaturaService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id:int}/profesores-count")]
        public async Task<IActionResult> GetProfesoresCount(int id)
        {
            var result = await _asignaturaService.GetProfesoresCountAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
