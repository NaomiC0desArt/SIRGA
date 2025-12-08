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
    public class GradoService : BaseService<Grado, GradoDto, GradoResponseDto>, IGradoService
    {
        public GradoService(
            IGradoRepository gradoRepository,
            ILogger<GradoService> logger)
            : base(gradoRepository, logger)
        {
        }

        protected override string EntityName => "Grado";

        #region Mapeos
        protected override Grado MapDtoToEntity(GradoDto dto)
        {
            return new Grado
            {
                GradeName = dto.GradeName,
                Section = dto.Section,
                StudentsLimit = dto.StudentsLimit
            };
        }

        protected override GradoResponseDto MapEntityToResponse(Grado entity)
        {
            return new GradoResponseDto
            {
                Id = entity.Id,
                GradeName = entity.GradeName,
                Section = entity.Section,
                StudentsLimit = entity.StudentsLimit
            };
        }

        protected override void UpdateEntityFromDto(Grado entity, GradoDto dto)
        {
            entity.GradeName = dto.GradeName;
            entity.Section = dto.Section;
            entity.StudentsLimit = dto.StudentsLimit;
        }
        #endregion
    }
}
