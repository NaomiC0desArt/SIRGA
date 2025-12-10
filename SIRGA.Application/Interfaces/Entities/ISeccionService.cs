using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface ISeccionService : IBaseServices<CreateSeccionDto, SeccionDto>
    {
        Task<ApiResponse<SeccionDto>> CrearSeccionAutomaticaAsync(int capacidadMaxima = 25);
    }
}
