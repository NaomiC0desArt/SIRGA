using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Application.Services
{
    public class PeriodoService(IPeriodoRepository periodoRepository, ILogger<PeriodoService> logger,
        ApplicationDbContext applicationDbContext)
        : IPeriodoService
    {
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;
        private readonly ILogger<PeriodoService> _logger = logger;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<ApiResponse<PeriodoResponseDto>> CreateAsync(PeriodoDto dto)
        {
            try
            {
                var periodo = new Periodo
                {
                    Numero = dto.Numero,
                    FechaInicio = dto.FechaInicio,
                    FechaFin = dto.FechaFin,
                    AnioEscolarId = dto.AnioEscolarId
                };

                var periodoCreado = await _periodoRepository.AddAsync(periodo);
                //Obtener el Año Escolar
                var anioEscolar = await _applicationDbContext.AniosEscolares
                    .FirstOrDefaultAsync(a => a.Id == dto.AnioEscolarId);

                var periodoResponse = new PeriodoResponseDto
                {
                    Numero = periodoCreado.Numero,
                    FechaInicio = periodoCreado.FechaInicio,
                    FechaFin = periodoCreado.FechaFin,
                    AnioEscolarId = periodoCreado.AnioEscolarId,
                    AnioEscolar = $"{anioEscolar.AnioInicio}-{anioEscolar.AnioFin}"
                };

                return ApiResponse<PeriodoResponseDto>.SuccessResponse(periodoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el periodo");
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "Error al crear le periodo",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var periodo = await _periodoRepository.GetByIdAsync(id);
                if (periodo == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Periodo no encontrado");
                }

                await _periodoRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el periodo");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar el periodo",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<PeriodoResponseDto>>> GetAllAsync()
        {
            try
            {
                var periodos = await _periodoRepository.GetAllAsync();
                _logger.LogInformation($"Periodos obtenidos: {periodos.Count}");

                var periodoResponse = new List<PeriodoResponseDto>();

                foreach (var periodo in periodos)
                {
                    _logger.LogInformation($"Periodo: {periodo.Id}");

                    //Obtener el Año Escolar
                    var anioEscolar = await _applicationDbContext.AniosEscolares
                        .FirstOrDefaultAsync(a => a.Id == periodo.AnioEscolarId);

                    periodoResponse.Add(new PeriodoResponseDto
                    {
                        Id = periodo.Id,
                        Numero = periodo.Numero,
                        FechaInicio = periodo.FechaInicio,
                        FechaFin = periodo.FechaFin,
                        AnioEscolarId = periodo.AnioEscolarId,
                        AnioEscolar = $"{anioEscolar.AnioInicio}-{anioEscolar.AnioFin}"
                    });
                }

                _logger.LogInformation("Periodos obtenidos exitosamente");
                return ApiResponse<List<PeriodoResponseDto>>.SuccessResponse(periodoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener los periodos", ex.ToString());
                return ApiResponse<List<PeriodoResponseDto>>.ErrorResponse("Todos los periodos no pudieron ser obtenidos");
            }
        }

        public async Task<ApiResponse<PeriodoResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var periodo = await _periodoRepository.GetByIdAsync(id);
                if (periodo == null)
                {
                    return ApiResponse<PeriodoResponseDto>.ErrorResponse("Periodo no encontrado");
                }
                //Obtener el Año Escolar
                var anioEscolar = await _applicationDbContext.AniosEscolares
                    .FirstOrDefaultAsync(a => a.Id == periodo.AnioEscolarId);

                var periodoResponse = new PeriodoResponseDto
                {
                    Id = periodo.Id,
                    Numero = periodo.Numero,
                    FechaInicio = periodo.FechaInicio,
                    FechaFin = periodo.FechaFin,
                    AnioEscolarId = periodo.AnioEscolarId,
                    AnioEscolar = $"{anioEscolar.AnioInicio}-{anioEscolar.AnioFin}"
                };

                return ApiResponse<PeriodoResponseDto>.SuccessResponse(periodoResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "Error al obtener el periodo",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<PeriodoResponseDto>> UpdateAsync(int id, PeriodoDto dto)
        {
            try
            {
                var periodo = await _periodoRepository.GetByIdAsync(id);
                if(periodo == null)
                {
                    return ApiResponse<PeriodoResponseDto>.ErrorResponse("Periodo no encontrado");
                }

                periodo.Id = dto.Id;
                periodo.Numero = dto.Numero;
                periodo.FechaInicio = dto.FechaInicio;
                periodo.FechaFin = dto.FechaFin;
                periodo.AnioEscolarId = dto.AnioEscolarId;

                var periodoUpdate = await _periodoRepository.UpdateAsync(periodo);
                //Obtener el Año Escolar
                var anioEscolar = await _applicationDbContext.AniosEscolares
                    .FirstOrDefaultAsync(a => a.Id == periodo.AnioEscolarId);

                var periodoResponse = new PeriodoResponseDto
                {
                    Id = periodoUpdate.Id,
                    Numero = periodoUpdate.Numero,
                    FechaInicio = periodoUpdate.FechaInicio,
                    FechaFin = periodoUpdate.FechaFin,
                    AnioEscolarId = periodoUpdate.AnioEscolarId,
                    AnioEscolar = $"{anioEscolar.AnioInicio}-{anioEscolar.AnioFin}"
                };

                return ApiResponse<PeriodoResponseDto>.SuccessResponse(periodoResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "Error al actualizar el periodo",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
