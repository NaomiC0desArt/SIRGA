using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.API.Controllers.Base;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services;
using SIRGA.Domain.Entities;

namespace SIRGA.API.Controllers.System
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CursoAcademicoController : BaseApiController<CursoAcademicoDto, CursoAcademicoResponseDto>
    {
        public CursoAcademicoController(ICursoAcademicoService service) : base(service)
        {
        }

        protected override string EntityRouteName => string.Empty;
    }
}

