

using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Estudiante;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IHorarioEstudianteService
    {
        Task<ApiResponse<HorarioSemanalDto>> GetHorarioByEstudianteIdAsync(int estudianteId);
        Task<ApiResponse<List<ClaseHorarioDto>>> GetClasesDelDiaAsync(int estudianteId, DayOfWeek diaSemana);
    }
}
