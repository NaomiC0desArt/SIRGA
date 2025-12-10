using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IGradoService : IBaseServices<CreateGradoDto, GradoDto> { }
}
