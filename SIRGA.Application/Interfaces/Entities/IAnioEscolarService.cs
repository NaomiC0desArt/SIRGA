using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IAnioEscolarService : IBaseServices<AnioEscolarDto, AnioEscolarDto>
    {
        Task<ApiResponse<AnioEscolarDto>> GetAnioActivoAsync();

    }
}
