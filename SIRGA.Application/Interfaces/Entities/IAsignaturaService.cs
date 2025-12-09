using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Asigantura;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IAsignaturaService : IBaseServices<AsignaturaDto, AsignaturaResponseDto>
    {
        Task<ApiResponse<int>> GetProfesoresCountAsync(int asignaturaId);
    }
}
