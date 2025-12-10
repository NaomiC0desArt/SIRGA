using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class AnioEscolarService : BaseService<AnioEscolar, AnioEscolarDto, AnioEscolarDto>, IAnioEscolarService
    {
        private readonly IAnioEscolarRepository _anioEscolarRepository;

        public AnioEscolarService(
            IAnioEscolarRepository anioEscolarRepository,
            ILogger<AnioEscolarService> logger)
            : base(anioEscolarRepository, logger)
        {
            _anioEscolarRepository = anioEscolarRepository;
        }

        protected override string EntityName => "Año Escolar";

        protected override AnioEscolar MapDtoToEntity(AnioEscolarDto dto)
        {
            return new AnioEscolar
            {
                AnioInicio = dto.AnioInicio,
                AnioFin = dto.AnioFin,
                Activo = dto.Activo
            };
        }

        protected override AnioEscolarDto MapEntityToResponse(AnioEscolar entity)
        {
            return new AnioEscolarDto
            {
                Id = entity.Id,
                AnioInicio = entity.AnioInicio,
                AnioFin = entity.AnioFin,
                Activo = entity.Activo,
                Periodo = entity.Periodo
            };
        }

        protected override void UpdateEntityFromDto(AnioEscolar entity, AnioEscolarDto dto)
        {
            entity.Activo = dto.Activo;
        }

        protected override async Task<ApiResponse<AnioEscolarDto>> ValidateCreateAsync(AnioEscolarDto dto)
        {
            if (dto.AnioFin <= dto.AnioInicio)
            {
                return ApiResponse<AnioEscolarDto>.ErrorResponse(
                    "El año de fin debe ser mayor al año de inicio");
            }

            if (dto.Activo)
            {
                await _anioEscolarRepository.DesactivarTodosAsync();
            }

            return null;
        }

        protected override async Task<ApiResponse<AnioEscolarDto>> ValidateUpdateAsync(int id, AnioEscolarDto dto)
        {
            var entity = await _anioEscolarRepository.GetByIdAsync(id);

            if (dto.Activo && !entity.Activo)
            {
                await _anioEscolarRepository.DesactivarTodosAsync();
            }

            return null;
        }

        // Método específico de AnioEscolar
        public async Task<ApiResponse<AnioEscolarDto>> GetAnioActivoAsync()
        {
            try
            {
                var anio = await _anioEscolarRepository.GetAnioActivoAsync();

                if (anio == null)
                    return ApiResponse<AnioEscolarDto>.ErrorResponse("No hay año escolar activo");

                var response = MapEntityToResponse(anio);
                return ApiResponse<AnioEscolarDto>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener año activo");
                return ApiResponse<AnioEscolarDto>.ErrorResponse(
                    "Error al obtener el año escolar activo",
                    new List<string> { ex.Message });
            }
        }
    }
}
