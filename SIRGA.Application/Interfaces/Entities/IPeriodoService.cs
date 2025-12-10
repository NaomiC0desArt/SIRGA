using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Periodo;
using SIRGA.Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Interfaces.Entities
{
    public interface IPeriodoService : IBaseServices<PeriodoDto, PeriodoResponseDto>
    {
        Task<ApiResponse<List<PeriodoResponseDto>>> GetByAnioEscolarAsync(int anioEscolarId);
        Task<ApiResponse<PeriodoActivoDto>> GetPeriodoActivoAsync();
    }
}
