using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class CursoAcademicoService : BaseService<CursoAcademico, CursoAcademicoDto, CursoAcademicoResponseDto>, ICursoAcademicoService
    {
        public CursoAcademicoService(
            ICursoAcademicoRepository cursoAcademicoRepository,
            ILogger<CursoAcademicoService> logger)
            : base(cursoAcademicoRepository, logger)
        {
        }

        protected override string EntityName => "Curso Académico";

        #region Mapeos
        protected override CursoAcademico MapDtoToEntity(CursoAcademicoDto dto)
        {
            return new CursoAcademico
            {
                IdGrado = dto.IdGrado,
                SchoolYear = dto.SchoolYear
            };
        }

        protected override CursoAcademicoResponseDto MapEntityToResponse(CursoAcademico entity)
        {
            return new CursoAcademicoResponseDto
            {
                Id = entity.Id,
                IdGrado = entity.IdGrado,
                Grado = entity.Grado,
                SchoolYear = entity.SchoolYear
            };
        }

        protected override void UpdateEntityFromDto(CursoAcademico entity, CursoAcademicoDto dto)
        {
            entity.IdGrado = dto.IdGrado;
            entity.SchoolYear = dto.SchoolYear;
        }
        #endregion
    }
}
