using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Services
{
    public class PeriodoService : BaseService<Periodo, PeriodoDto, PeriodoResponseDto>, IPeriodoService
    {
        private readonly IPeriodoRepository _periodoRepository;

        public PeriodoService(
            IPeriodoRepository periodoRepository,
            ILogger<PeriodoService> logger)
            : base(periodoRepository, logger)
        {
            _periodoRepository = periodoRepository;
        }

        protected override string EntityName => "Periodo";

        protected override Periodo MapDtoToEntity(PeriodoDto dto)
        {
            return new Periodo
            {
                Numero = dto.Numero,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                AnioEscolarId = dto.AnioEscolarId
            };
        }

        protected override PeriodoResponseDto MapEntityToResponse(Periodo entity)
        {
            return new PeriodoResponseDto
            {
                Id = entity.Id,
                Numero = entity.Numero,
                FechaInicio = entity.FechaInicio,
                FechaFin = entity.FechaFin,
                AnioEscolarId = entity.AnioEscolarId,
                AnioEscolar = entity.AnioEscolar?.Periodo,
                NombrePeriodo = entity.NombrePeriodo,
                DuracionDias = entity.DuracionDias
            };
        }

        protected override void UpdateEntityFromDto(Periodo entity, PeriodoDto dto)
        {
            entity.Numero = dto.Numero;
            entity.FechaInicio = dto.FechaInicio;
            entity.FechaFin = dto.FechaFin;
            entity.AnioEscolarId = dto.AnioEscolarId;
        }

        protected override async Task<ApiResponse<PeriodoResponseDto>> ValidateCreateAsync(PeriodoDto dto)
        {
            if (dto.FechaFin <= dto.FechaInicio)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "La fecha de fin debe ser posterior a la fecha de inicio");
            }

            var existe = await _periodoRepository.ExistePeriodoAsync(dto.Numero, dto.AnioEscolarId);
            if (existe)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    $"Ya existe el periodo {dto.Numero} para este año escolar");
            }

            return null;
        }

        protected override async Task<ApiResponse<PeriodoResponseDto>> ValidateUpdateAsync(int id, PeriodoDto dto)
        {
            if (dto.FechaFin <= dto.FechaInicio)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "La fecha de fin debe ser posterior a la fecha de inicio");
            }

            return null;
        }

        protected override async Task<Periodo> AfterCreateAsync(Periodo entity)
        {
            return await _periodoRepository.GetByIdWithAnioEscolarAsync(entity.Id);
        }

        protected override async Task<Periodo> AfterUpdateAsync(Periodo entity)
        {
            return await _periodoRepository.GetByIdWithAnioEscolarAsync(entity.Id);
        }

        // Método específico de Periodo
        public async Task<ApiResponse<List<PeriodoResponseDto>>> GetByAnioEscolarAsync(int anioEscolarId)
        {
            try
            {
                var periodos = await _periodoRepository.GetByAnioEscolarAsync(anioEscolarId);
                var response = periodos.Select(MapEntityToResponse).ToList();

                return ApiResponse<List<PeriodoResponseDto>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener periodos por año escolar");
                return ApiResponse<List<PeriodoResponseDto>>.ErrorResponse(
                    "Error al obtener los periodos",
                    new List<string> { ex.Message });
            }
        }
    }
}
