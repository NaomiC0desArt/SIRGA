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
    public class SeccionController : BaseApiController<CreateSeccionDto, SeccionDto>
    {
        private readonly ISeccionService _seccionService;

        public SeccionController(ISeccionService service) : base(service)
        {
            _seccionService = service;
        }

        protected override string EntityRouteName => string.Empty;


        [HttpPost("CrearAutomatica")]
        public async Task<IActionResult> CrearAutomatica()
        {
            var result = await _seccionService.CrearSeccionAutomaticaAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

