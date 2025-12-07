using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class AsignaturaService : BaseService<Asignatura, AsignaturaDto, AsignaturaResponseDto>, IAsignaturaService
    {
        private readonly IAsignaturaRepository _asignaturaRepository;

        public AsignaturaService(
            IAsignaturaRepository asignaturaRepository,
            ILogger<AsignaturaService> logger)
            : base(asignaturaRepository, logger)
        {
            _asignaturaRepository = asignaturaRepository;
        }

        protected override string EntityName => "Asignatura";
        
        #region Mapeos
        protected override Asignatura MapDtoToEntity(AsignaturaDto dto)
        {
            return new Asignatura
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };
        }

        protected override AsignaturaResponseDto MapEntityToResponse(Asignatura entity)
        {
            return new AsignaturaResponseDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion
            };
        }

        protected override void UpdateEntityFromDto(Asignatura entity, AsignaturaDto dto)
        {
            entity.Nombre = dto.Nombre;
            entity.Descripcion = dto.Descripcion;
        }
        #endregion

        #region Validaciones
        protected override Task<ApiResponse<AsignaturaResponseDto>> ValidateCreateAsync(AsignaturaDto dto)
        {
            return ValidateDescripcion(dto);
        }

        protected override Task<ApiResponse<AsignaturaResponseDto>> ValidateUpdateAsync(int id, AsignaturaDto dto)
        {
            return ValidateDescripcion(dto);
        }

        private Task<ApiResponse<AsignaturaResponseDto>> ValidateDescripcion(AsignaturaDto dto)
        {
            if (dto.Descripcion?.Length > 125)
            {
                return Task.FromResult(
                    ApiResponse<AsignaturaResponseDto>.ErrorResponse(
                        "La descripción no puede exceder los 125 caracteres"
                    )
                );
            }
            return Task.FromResult<ApiResponse<AsignaturaResponseDto>>(null);
        }
        #endregion

        #region Métodos Específicos
        public async Task<ApiResponse<int>> GetProfesoresCountAsync(int asignaturaId)
        {
            try
            {
                var asignatura = await _asignaturaRepository.GetByIdAsync(asignaturaId);
                if (asignatura == null)
                {
                    return ApiResponse<int>.ErrorResponse("Asignatura no encontrada");
                }

                var count = await _asignaturaRepository.GetProfesoresCountAsync(asignaturaId);
                return ApiResponse<int>.SuccessResponse(
                    count,
                    "Cantidad de profesores obtenida exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cantidad de profesores");
                return ApiResponse<int>.ErrorResponse(
                    "Error al obtener cantidad de profesores"
                );
            }
        }
        #endregion
    }
}
