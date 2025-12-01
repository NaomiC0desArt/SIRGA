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
        private static Random _random = new Random();

        public AsignaturaService(IAsignaturaRepository asignaturaRepository, ILogger<AsignaturaService> logger)
        {
            _asignaturaRepository = asignaturaRepository;
            _logger = logger;
        }

        private static string GenerarCodigoAsignatura(string nombreAsignatura)
        {
            // Quitar espacios
            string limpio = nombreAsignatura.Replace(" ", "");

            if (limpio.Length < 3)
                throw new ArgumentException("El nombre de la asignatura debe tener al menos 3 letras.");

            // Tomar primeras 3 letras en mayúscula
            string letras = limpio.Substring(0, 3).ToUpper();

            // Generar número aleatorio de 3 dígitos
            int numero = _random.Next(100, 1000); // 100 hasta 999

            return $"{letras}{numero}";
        }

        public async Task<ApiResponse<AsignaturaResponseDto>> CreateAsync(AsignaturaDto dto)
        {
            try
            {
                var codigoAsignatura = GenerarCodigoAsignatura(dto.Nombre);

                var response = new Asignatura
                {
                    Nombre = dto.Nombre,
                    Codigo = codigoAsignatura,
                    Descripcion = dto.Descripcion,
                    TipoAsignatura = dto.TipoAsignatura
                };

                await _asignaturaRepository.AddAsync(response);

                var asignaturaResponse = new AsignaturaResponseDto
                {
                    Nombre = response.Nombre,
                    Codigo = response.Codigo,
                    Descripcion = response.Descripcion,
                    TipoAsignatura = response.TipoAsignatura.ToString()
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
                        Codigo = asignatura.Codigo,
                        Descripcion = asignatura.Descripcion,
                        TipoAsignatura = asignatura.TipoAsignatura.ToString()
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
                    Codigo = asignatura.Codigo,
                    Descripcion = asignatura.Descripcion,
                    TipoAsignatura = asignatura.TipoAsignatura.ToString()
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
                asignatura.Codigo = dto.Codigo;
                asignatura.Descripcion = dto.Descripcion;
                asignatura.TipoAsignatura = dto.TipoAsignatura;

                await _asignaturaRepository.UpdateAsync(asignatura);

                var asignaturaResponse = new AsignaturaResponseDto
                {
                    Id = asignatura.Id,
                    Nombre = asignatura.Nombre,
                    Codigo = asignatura.Codigo,
                    Descripcion = asignatura.Descripcion,
                    TipoAsignatura = asignatura.TipoAsignatura.ToString()
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
