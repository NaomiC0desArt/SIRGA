using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities.Asigantura;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AsignaturaController : BaseApiController<AsignaturaDto, AsignaturaResponseDto>
    {
        private readonly IAsignaturaService _asignaturaService;

        public AsignaturaController(IAsignaturaService service) : base(service)
        {
            _asignaturaService = service;
        }

        protected override string EntityRouteName => string.Empty;

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
