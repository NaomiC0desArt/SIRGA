using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClaseProgramadaController : BaseApiController<ClaseProgramadaDto, ClaseProgramadaResponseDto>
    { 
            public ClaseProgramadaController(IClaseProgramadaService service) : base(service)
            {
            }

            protected override string EntityRouteName => string.Empty;
    }
    }

