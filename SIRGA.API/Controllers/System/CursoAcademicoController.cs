using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services;
using SIRGA.Domain.Entities;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursoAcademicoController : ControllerBase
    {
        private readonly ICursoAcademicoService _cursoAcademicoService;

        public CursoAcademicoController(ICursoAcademicoService cursoAcademicoService)
        {
            _cursoAcademicoService = cursoAcademicoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cursoAcademicoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetCursoAcademicoById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _cursoAcademicoService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Create([FromBody] CursoAcademicoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cursoAcademicoService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetCursoAcademicoById", new { id = result.Data.Id }, result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CursoAcademicoDto dto)
        {
            var result = await _cursoAcademicoService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cursoAcademicoService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
