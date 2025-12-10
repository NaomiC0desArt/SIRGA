using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Enum;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class GradoService : BaseService<Grado, CreateGradoDto, GradoDto>, IGradoService
    {
        public GradoService(
            IGradoRepository gradoRepository,
            ILogger<GradoService> logger)
            : base(gradoRepository, logger)
        {
        }

        protected override string EntityName => "Grado";

        protected override Grado MapDtoToEntity(CreateGradoDto dto)
        {
            return new Grado
            {
                GradeName = dto.GradeName,
                Nivel = (NivelEducativo)dto.Nivel
            };
        }

        protected override GradoDto MapEntityToResponse(Grado entity)
        {
            return new GradoDto
            {
                Id = entity.Id,
                GradeName = entity.GradeName,
                Nivel = entity.Nivel.ToString()
            };
        }

        protected override void UpdateEntityFromDto(Grado entity, CreateGradoDto dto)
        {
            entity.GradeName = dto.GradeName;
            entity.Nivel = (NivelEducativo)dto.Nivel;
        }
    }
}
