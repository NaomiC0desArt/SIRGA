using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface ICursoAcademicoService : IBaseServices<CreateCursoAcademicoDto, CursoAcademicoDto>
    {
        Task<ApiResponse<List<SeccionDto>>> GetSeccionesDisponiblesAsync(int idGrado, int idAnioEscolar);
    }
}
