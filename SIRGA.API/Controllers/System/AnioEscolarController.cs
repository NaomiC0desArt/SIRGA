using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AnioEscolarController : BaseApiController<AnioEscolarDto, AnioEscolarDto>
    {
        private readonly IAnioEscolarService _anioService;

        public AnioEscolarController(IAnioEscolarService service) : base(service)
        {
            _anioService = service;
        }

        protected override string EntityRouteName => string.Empty;

        [HttpGet("Activo")]
        public async Task<IActionResult> GetAnioActivo()
        {
            var result = await _anioService.GetAnioActivoAsync();

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}

