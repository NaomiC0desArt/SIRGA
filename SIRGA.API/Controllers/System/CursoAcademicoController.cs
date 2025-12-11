using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services;
using SIRGA.Domain.Entities;

namespace SIRGA.API.Controllers.System
{
    [Authorize(Roles = "Admin,Profesor")]
    [Route("api/[controller]")]
    [ApiController]
    public class CursoAcademicoController : BaseApiController<CreateCursoAcademicoDto, CursoAcademicoDto>
    {
        private readonly ICursoAcademicoService _cursoService;

        public CursoAcademicoController(ICursoAcademicoService service) : base(service)
        {
            _cursoService = service;
        }

        protected override string EntityRouteName => string.Empty;

        [HttpGet("SeccionesDisponibles")]
        public async Task<IActionResult> GetSeccionesDisponibles([FromQuery] int idGrado, [FromQuery] int idAnioEscolar)
        {
            var result = await _cursoService.GetSeccionesDisponiblesAsync(idGrado, idAnioEscolar);
            return Ok(result);
        }
    }
}

