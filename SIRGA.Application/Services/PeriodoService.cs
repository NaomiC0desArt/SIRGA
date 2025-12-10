using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Periodo;
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
            var hoy = DateTime.Today;
            var esActivo = entity.FechaInicio <= hoy && entity.FechaFin >= hoy;
            var duracionDias = (entity.FechaFin - entity.FechaInicio).Days + 1;
            var duracionSemanas = (int)Math.Ceiling(duracionDias / 7.0);

            return new PeriodoResponseDto
            {
                Id = entity.Id,
                Numero = entity.Numero,
                FechaInicio = entity.FechaInicio,
                FechaFin = entity.FechaFin,
                AnioEscolarId = entity.AnioEscolarId,
                AnioEscolar = entity.AnioEscolar?.Periodo,
                NombrePeriodo = entity.NombrePeriodo,
                DuracionDias = duracionDias,
                DuracionSemanas = duracionSemanas,
                EsActivo = esActivo,
                PuedeEditar = !esActivo || entity.FechaFin > hoy
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
            // Validar fechas
            if (dto.FechaFin <= dto.FechaInicio)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "La fecha de fin debe ser posterior a la fecha de inicio");
            }

            // Calcular duración en semanas
            var duracionDias = (dto.FechaFin - dto.FechaInicio).Days + 1;
            var duracionSemanas = (int)Math.Ceiling(duracionDias / 7.0);

            // Validar duración mínima (6 semanas)
            if (duracionSemanas < 6)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    $"El período debe tener una duración mínima de 6 semanas. Duración actual: {duracionSemanas} semanas");
            }

            // Validar duración máxima (14 semanas)
            if (duracionSemanas > 14)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    $"El período no puede exceder las 14 semanas de duración. Duración actual: {duracionSemanas} semanas");
            }

            // Validar que el número esté entre 1 y 4
            if (dto.Numero < 1 || dto.Numero > 4)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "El número de período debe estar entre 1 y 4");
            }

            // Validar que no exista ya ese período para el año escolar
            var existe = await _periodoRepository.ExistePeriodoAsync(dto.Numero, dto.AnioEscolarId);
            if (existe)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    $"Ya existe el período {dto.Numero} para este año escolar");
            }

            // Validar que no haya más de 4 períodos por año escolar
            var cantidadPeriodos = await _periodoRepository.ContarPeriodosPorAnioAsync(dto.AnioEscolarId);
            if (cantidadPeriodos >= 4)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "No se pueden crear más de 4 períodos por año escolar");
            }

            return null;
        }

        protected override async Task<ApiResponse<PeriodoResponseDto>> ValidateUpdateAsync(int id, PeriodoDto dto)
        {
            // Validar fechas
            if (dto.FechaFin <= dto.FechaInicio)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    "La fecha de fin debe ser posterior a la fecha de inicio");
            }

            // Calcular duración en semanas
            var duracionDias = (dto.FechaFin - dto.FechaInicio).Days + 1;
            var duracionSemanas = (int)Math.Ceiling(duracionDias / 7.0);

            // Validar duración mínima (6 semanas)
            if (duracionSemanas < 6)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    $"El período debe tener una duración mínima de 6 semanas. Duración actual: {duracionSemanas} semanas");
            }

            // Validar duración máxima (14 semanas)
            if (duracionSemanas > 14)
            {
                return ApiResponse<PeriodoResponseDto>.ErrorResponse(
                    $"El período no puede exceder las 14 semanas de duración. Duración actual: {duracionSemanas} semanas");
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

        public async Task<ApiResponse<PeriodoActivoDto>> GetPeriodoActivoAsync()
        {
            try
            {
                var periodo = await _periodoRepository.GetPeriodoActivoAsync();

                if (periodo == null)
                {
                    return ApiResponse<PeriodoActivoDto>.ErrorResponse(
                        "No hay período activo en este momento");
                }

                var hoy = DateTime.Today;
                var diasRestantes = (periodo.FechaFin - hoy).Days;

                var response = new PeriodoActivoDto
                {
                    Id = periodo.Id,
                    Numero = periodo.Numero,
                    FechaInicio = periodo.FechaInicio,
                    FechaFin = periodo.FechaFin,
                    NombrePeriodo = periodo.NombrePeriodo,
                    DiasRestantes = diasRestantes,
                    AnioEscolar = periodo.AnioEscolar?.Periodo
                };

                return ApiResponse<PeriodoActivoDto>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período activo");
                return ApiResponse<PeriodoActivoDto>.ErrorResponse(
                    "Error al obtener el período activo",
                    new List<string> { ex.Message });
            }
        }
    }
}
