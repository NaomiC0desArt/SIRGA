using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodoController : BaseApiController<PeriodoDto, PeriodoResponseDto>
    {
        private readonly IPeriodoService _periodoService;

        public PeriodoController(IPeriodoService service) : base(service)
        {
            _periodoService = service;
        }

        protected override string EntityRouteName => "Periodo";

        // Endpoint específico
        [HttpGet("ByAnioEscolar/{anioEscolarId:int}")]
        public async Task<IActionResult> GetByAnioEscolar(int anioEscolarId)
        {
            var result = await _periodoService.GetByAnioEscolarAsync(anioEscolarId);
            return Ok(result);
        }
    }
}
