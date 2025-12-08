using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    /// <summary>
    /// Cabe aclarar que al igual que los demás métodos no serán implementados en Api,
    /// solo en los métodos de Calificación, ya que de las calificaciones es que 
    /// dependen los campos de esta entidad.
    /// Ejemplo: Cuando se crée las calificaciones del P1, se implementa el método de crear
    /// de esta entidad para que guarde el total de las notas de lso periodos.
    /// </summary>
    /*public class AnnualGradeService(IAnnualGrateRepository annualGrateRepository, ILogger<AnnualGradeService> logger)
        : IAnnualGradeService
    {
        private readonly IAnnualGrateRepository _annualGrateRepository = annualGrateRepository;
        private readonly ILogger<AnnualGradeService> _logger = logger;

        /// <summary>
        /// Aquí se guardan llas notas de cada periodo junto con el total del promedio de las notas.
        /// </summary>
        public async Task<ApiResponse<AnnualGradeResponseDto>> CreateAsync(AnnualGradeDto dto)
        {
            try
            {
                var response = new AnnualGrade
                {
                    CalificacionId = dto.CalificacionId,
                    PeriodoId = dto.PeriodoId,
                    P1 = dto?.P1,
                    P2 = dto?.P2,
                    P3 = dto?.P3,
                    P4 = dto?.P4,
                    Total = dto.Total
                };

                await _annualGrateRepository.AddAsync(response);

                var annualGradeResponse = new AnnualGradeResponseDto
                {
                    CalificacionId = response.CalificacionId,
                    PeriodoId = response.PeriodoId,
                    P1 = response?.P1,
                    P2 = response?.P2,
                    P3 = response?.P3,
                    P4 = response?.P4,
                    Total = response.Total
                };

                return ApiResponse<AnnualGradeResponseDto>.SuccessResponse(annualGradeResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<AnnualGradeResponseDto>.ErrorResponse("Error al guardar La calificacion del periodo");
            }
        }

        public Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<AnnualGradeResponseDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<AnnualGradeResponseDto>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<AnnualGradeResponseDto>> UpdateAsync(int id, AnnualGradeDto dto)
        {
            throw new NotImplementedException();
        }
    }*/
}
