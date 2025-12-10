using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class SeccionService : BaseService<Seccion, CreateSeccionDto, SeccionDto>, ISeccionService
    {
        private readonly ISeccionRepository _seccionRepository;

        public SeccionService(
            ISeccionRepository seccionRepository,
            ILogger<SeccionService> logger)
            : base(seccionRepository, logger)
        {
            _seccionRepository = seccionRepository;
        }

        protected override string EntityName => "Sección";

        protected override Seccion MapDtoToEntity(CreateSeccionDto dto)
        {
            return new Seccion
            {
                Nombre = dto.Nombre.ToUpper(),
                CapacidadMaxima = dto.CapacidadMaxima
            };
        }

        protected override SeccionDto MapEntityToResponse(Seccion entity)
        {
            return new SeccionDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                CapacidadMaxima = entity.CapacidadMaxima
            };
        }

        protected override void UpdateEntityFromDto(Seccion entity, CreateSeccionDto dto)
        {
            entity.Nombre = dto.Nombre.ToUpper();
            entity.CapacidadMaxima = dto.CapacidadMaxima;
        }

        protected override async Task<ApiResponse<SeccionDto>> ValidateCreateAsync(CreateSeccionDto dto)
        {
            var existe = await _seccionRepository.ExisteSeccionAsync(dto.Nombre.ToUpper());
            if (existe)
            {
                return ApiResponse<SeccionDto>.ErrorResponse(
                    $"Ya existe una sección con el nombre '{dto.Nombre}'");
            }
            return null;
        }

        public async Task<ApiResponse<SeccionDto>> CrearSeccionAutomaticaAsync(int capacidadMaxima = 25)
        {
            try
            {
                var proximaLetra = await _seccionRepository.GetProximaLetraDisponibleAsync();

                var nuevaSeccion = new CreateSeccionDto
                {
                    Nombre = proximaLetra,
                    CapacidadMaxima = 30 // Capacidad por defecto
                };

                return await CreateAsync(nuevaSeccion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear sección automática");
                return ApiResponse<SeccionDto>.ErrorResponse(
                    "Error al crear la sección automáticamente",
                    new List<string> { ex.Message });
            }
        }
    }
}
