using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Enum;

namespace SIRGA.API.Controllers.System
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AulaController : BaseApiController<CreateAulaDto, AulaDto>
    {
        private readonly IAulaService _aulaService;

        public AulaController(IAulaService service) : base(service)
        {
            _aulaService = service;
        }

        protected override string EntityRouteName => "Aula";

        // Endpoint específico
        [HttpGet("Disponibles")]
        public async Task<IActionResult> GetDisponibles()
        {
            var result = await _aulaService.GetAulasDisponiblesAsync();
            return Ok(result);
        }

        // Override para usar UpdateAulaDto
        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> UpdateAula(int id, [FromBody] UpdateAulaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _aulaService.UpdateAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GenerarCodigo")]
        public async Task<IActionResult> GenerarCodigo([FromQuery] int tipo, [FromQuery] string nombre = "")
        {
            var result = await _aulaService.GenerarCodigoAsync((TipoEspacio)tipo, nombre);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

