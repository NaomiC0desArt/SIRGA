using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class AsignaturaService : IAsignaturaService
    {
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly ILogger<AsignaturaService> _logger;

        public AsignaturaService(IAsignaturaRepository asignaturaRepository, ILogger<AsignaturaService> logger)
        {
            _asignaturaRepository = asignaturaRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<AsignaturaResponseDto>> CreateAsync(AsignaturaDto dto)
        {
            try
            {
                var response = new Asignatura
                {
                    Id = dto.Id,
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion
                };

                await _asignaturaRepository.AddAsync(response);

                var asignaturaResponse = new AsignaturaResponseDto
                {
                    Id = response.Id,
                    Nombre = response.Nombre,
                    Descripcion = response.Descripcion
                };

                return ApiResponse<AsignaturaResponseDto>.SuccessResponse(asignaturaResponse, "Asignatura creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear la asignatura", ex.ToString());
                return ApiResponse<AsignaturaResponseDto>.ErrorResponse("Error al crear la asignatura");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var asignatura = await _asignaturaRepository.GetByIdAsync(id);
                if (asignatura == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Asignatura no encontrada");
                }

                await _asignaturaRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true, "Asignatura eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la asignatura",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<AsignaturaResponseDto>>> GetAllAsync()
        {
            try
            {
                var asignaturas = await _asignaturaRepository.GetAllAsync();
                _logger.LogInformation($"Asignaturas obtenidas: {asignaturas.Count}");

                var asignaturaResponses = new List<AsignaturaResponseDto>();

                foreach (var asignatura in asignaturas)
                {
                    _logger.LogInformation($"Procesando asignatura: {asignatura.Nombre}");

                    asignaturaResponses.Add(new AsignaturaResponseDto
                    {
                        Id = asignatura.Id,
                        Nombre = asignatura.Nombre,
                        Descripcion = asignatura.Descripcion
                    });
                }

                _logger.LogInformation("Todas las asignaturas procesadas correctamente");
                return ApiResponse<List<AsignaturaResponseDto>>.SuccessResponse(asignaturaResponses, "Asignaturas obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener las asignaturas", ex.ToString());
                throw;
            }
        }

        public async Task<ApiResponse<AsignaturaResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var asignatura = await _asignaturaRepository.GetByIdAsync(id);
                if (asignatura == null)
                {
                    return ApiResponse<AsignaturaResponseDto>.ErrorResponse("Asignatura no encontrada");
                }

                var asignaturaResponse = new AsignaturaResponseDto
                {
                    Id = asignatura.Id,
                    Nombre = asignatura.Nombre,
                    Descripcion = asignatura.Descripcion
                };

                return ApiResponse<AsignaturaResponseDto>.SuccessResponse(asignaturaResponse, "Asignatura obtenida exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<AsignaturaResponseDto>.ErrorResponse(
                    "Error al obtener la asignatura",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<AsignaturaResponseDto>> UpdateAsync(int id, AsignaturaDto dto)
        {
            try
            {
                var asignatura = await _asignaturaRepository.GetByIdAsync(id);
                if (asignatura == null)
                {
                    return ApiResponse<AsignaturaResponseDto>.ErrorResponse("Asignatura no encontrada");
                }
                
                asignatura.Nombre = dto.Nombre;
                asignatura.Descripcion = dto.Descripcion;

                await _asignaturaRepository.UpdateAsync(asignatura);

                var asignaturaResponse = new AsignaturaResponseDto
                {
                    Id = asignatura.Id,
                    Nombre = asignatura.Nombre,
                    Descripcion = asignatura.Descripcion
                };

                return ApiResponse<AsignaturaResponseDto>.SuccessResponse(asignaturaResponse, "Asignatura actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<AsignaturaResponseDto>.ErrorResponse(
                    "Error al actualizar la asignatura",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
