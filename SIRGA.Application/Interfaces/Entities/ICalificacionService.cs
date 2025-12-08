using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface ICalificacionService : IBaseServices<CalificacionDto, CalificacionResponseDto>
    {
        public Task<ApiResponse<CalificacionResponseDto>> PublicarCalificacionAsync(int Id);
        public Task<ApiResponse<AnnualGradeDto>> GetAnnualGradesAsync(int estudianteId, int asignaturaId, int cursoId, int anioEscolarId);
    }
}
