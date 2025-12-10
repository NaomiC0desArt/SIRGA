using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Base;
using SIRGA.Domain.Enum;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IAulaService : IBaseServices<CreateAulaDto, AulaDto>
    {
        Task<ApiResponse<List<AulaDto>>> GetAulasDisponiblesAsync();
        Task<ApiResponse<AulaDto>> UpdateAsync(int id, UpdateAulaDto dto);
        Task<ApiResponse<string>> GenerarCodigoAsync(TipoEspacio tipo, string nombre);
    }
}
