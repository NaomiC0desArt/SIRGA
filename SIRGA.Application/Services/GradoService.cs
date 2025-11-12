using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class GradoService : IGradoService
    {
        private readonly IGradoRepository _gradoRepository;
        private readonly ILogger<GradoService> _logger;

        public GradoService(IGradoRepository gradoRepository, ILogger<GradoService> logger)
        {
            _gradoRepository = gradoRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<GradoResponseDto>> CreateAsync(GradoDto dto)
        {
            try
            {
                var response = new Grado
                {
                    Id = dto.Id,
                    GradeName = dto.GradeName,
                    Section = dto.Section,
                    StudentsLimit = dto.StudentsLimit
                };

                await _gradoRepository.AddAsync(response);

                var gradoResponse = new GradoResponseDto
                {
                    Id = response.Id,
                    GradeName = response.GradeName,
                    Section = response.Section,
                    StudentsLimit = response.StudentsLimit
                };

                return ApiResponse<GradoResponseDto>.SuccessResponse(gradoResponse, "Grado creado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<GradoResponseDto>.ErrorResponse(
                    "Error al crear el Grado",
                    new List<string> { ex.Message }
                    );
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var grado = await _gradoRepository.GetByIdAsync(id);
                if( grado == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Error encontrando el grado");
                }
                await _gradoRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true, "Grado eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar el grado",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<GradoResponseDto>>> GetAllAsync()
        {
            try
            {
                var grados = await _gradoRepository.GetAllAsync();
                _logger.LogInformation($"Grados obtenidos: {grados.Count}");

                var gradoResponse = new List<GradoResponseDto>();

                foreach ( var grado in grados)
                {
                    _logger.LogInformation($"Procesando grado: {grado.GradeName}");

                    gradoResponse.Add(new GradoResponseDto
                    {
                        Id = grado.Id,
                        GradeName = grado.GradeName,
                        Section = grado.Section,
                        StudentsLimit = grado.StudentsLimit
                    });
                }

                _logger.LogInformation("Todos los grados procesados exitosamente");
                return ApiResponse<List<GradoResponseDto>>.SuccessResponse(gradoResponse, "Grados obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<GradoResponseDto>>.ErrorResponse(
                    "Error al obtener los grados",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<GradoResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var grado = await _gradoRepository.GetByIdAsync(id);
                if ( grado == null)
                {
                    return ApiResponse<GradoResponseDto>.ErrorResponse("Grado no encontrado");
                }

                var gradoResponse = new GradoResponseDto
                {
                    Id = grado.Id,
                    GradeName = grado.GradeName,
                    Section = grado.Section,
                    StudentsLimit= grado.StudentsLimit
                };

                return ApiResponse<GradoResponseDto>.SuccessResponse(gradoResponse, "Grado obtenido exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<GradoResponseDto>.ErrorResponse(
                    "Error al obtener el grado",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<GradoResponseDto>> UpdateAsync(int id, GradoDto dto)
        {
            try
            {
                var grado = await _gradoRepository.GetByIdAsync(id);
                if( grado == null)
                {
                    return ApiResponse<GradoResponseDto>.ErrorResponse("Grano no encontrado");
                }

                grado.GradeName = dto.GradeName;
                grado.Section = dto.Section;
                grado.StudentsLimit = dto.StudentsLimit;

                await _gradoRepository.UpdateAsync(grado);

                var gradoResponse = new GradoResponseDto
                {
                    Id = grado.Id,
                    GradeName = grado.GradeName,
                    Section = grado.Section,
                    StudentsLimit = grado.StudentsLimit
                };

                return ApiResponse<GradoResponseDto>.SuccessResponse(gradoResponse, "Grado actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<GradoResponseDto>.ErrorResponse(
                    "Error al actualizar el grado",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
