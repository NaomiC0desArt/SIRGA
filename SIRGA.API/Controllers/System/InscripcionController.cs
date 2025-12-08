using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class InscripcionController : BaseApiController<InscripcionDto, InscripcionResponseDto>
    {
        public InscripcionController(IInscripcionService service) : base(service)
        {
        }

        protected override string EntityRouteName => string.Empty;
    }
}
