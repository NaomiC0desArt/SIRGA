using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class AnioEscolarService(IAnioEscolarRepository anioEscolarRepository, ILogger<AnioEscolarService> logger)
        : IAnioEscolarService
    {
        private readonly IAnioEscolarRepository _anioEscolarRepository = anioEscolarRepository;
        private readonly ILogger<AnioEscolarService> _logger = logger;

        public async Task<ApiResponse<AnioEscolarResponseDto>> CreateAsync(AnioEscolarDto dto)
        {
            try
            {
                var response = new AnioEscolar
                {
                    Id = dto.Id,
                    AnioInicio = dto.AnioInicio,
                    AnioFin = dto.AnioFin,
                    Activo = dto.Activo

                };

                await _anioEscolarRepository.AddAsync(response);

                var anioEscolarResponse = new AnioEscolarResponseDto
                {
                    Id = response.Id,
                    AnioInicio = response.AnioInicio,
                    AnioFin = response.AnioFin,
                    Activo = response.Activo
                };

                return ApiResponse<AnioEscolarResponseDto>.SuccessResponse(anioEscolarResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al realizar esta funcion", ex.ToString());
                return ApiResponse<AnioEscolarResponseDto>.ErrorResponse("Error al crear el año escolar");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var anioEscolar = await _anioEscolarRepository.GetByIdAsync(id);
                if( anioEscolar == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Año no encontrado");
                }
                
                await _anioEscolarRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true, "Acción realizada con éxito");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error realizando esta acción",
                    new List<string> {ex.Message});
            }
        }

        public async Task<ApiResponse<List<AnioEscolarResponseDto>>> GetAllAsync()
        {
            try
            {
                var anioEscolar = await _anioEscolarRepository.GetAllAsync();
                _logger.LogInformation($"Años escolares registrados {anioEscolar.Count}");

                var aniosResponse = new List<AnioEscolarResponseDto>();

                foreach (var anio in anioEscolar)
                {
                    _logger.LogInformation($"Registrando el año: {anio.Id}");

                    aniosResponse.Add(new AnioEscolarResponseDto()
                    {
                        Id = anio.Id,
                        AnioInicio = anio.AnioInicio,
                        AnioFin = anio.AnioFin,
                        Activo = anio.Activo
                    });
                }
                _logger.LogInformation("Todos los años escolares obtenidos");
                return ApiResponse<List<AnioEscolarResponseDto>>.SuccessResponse(aniosResponse, "Jevi");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener los años escolares", ex.ToString());
                return ApiResponse<List<AnioEscolarResponseDto>>.ErrorResponse("Todos los años escolares no pudieron ser obtenidos");
            }
        }

        public async Task<ApiResponse<AnioEscolarResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var anioEscolar = await _anioEscolarRepository.GetByIdAsync(id);
                if (anioEscolar == null)
                {
                    return ApiResponse<AnioEscolarResponseDto>.ErrorResponse("No se encuentra registro");
                }

                var anioResponse = new AnioEscolarResponseDto
                {
                    Id = anioEscolar.Id,
                    AnioInicio = anioEscolar.AnioInicio,
                    AnioFin = anioEscolar.AnioFin,
                    Activo = anioEscolar.Activo
                };

                return ApiResponse<AnioEscolarResponseDto>.SuccessResponse(anioResponse, "Jevi");
            }
            catch (Exception ex)
            {
                return ApiResponse<AnioEscolarResponseDto>.ErrorResponse(
                    "Error al obtener el registro",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<AnioEscolarResponseDto>> UpdateAsync(int id, AnioEscolarDto dto)
        {
            try
            {
                var anioEscolar = await _anioEscolarRepository.GetByIdAsync(id);
                if(anioEscolar == null)
                {
                    return ApiResponse<AnioEscolarResponseDto>.ErrorResponse("No se encontró registro");
                }

                anioEscolar.AnioInicio = dto.AnioInicio;
                anioEscolar.AnioFin = dto.AnioFin;
                anioEscolar.Activo = dto.Activo;

                await _anioEscolarRepository.UpdateAsync(anioEscolar);

                var anioEscolarResponse = new AnioEscolarResponseDto
                {
                    Id = anioEscolar.Id,
                    AnioInicio = anioEscolar.AnioInicio,
                    AnioFin = anioEscolar.AnioFin,
                    Activo = anioEscolar.Activo
                };

                return ApiResponse<AnioEscolarResponseDto>.SuccessResponse(anioEscolarResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<AnioEscolarResponseDto>.ErrorResponse(
                    "Error al actualizar",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
