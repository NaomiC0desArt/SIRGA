using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Asigantura;
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
        private static readonly Random _random = new Random();

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
                Descripcion = dto.Descripcion,
                TipoAsignatura = dto.TipoAsignatura
            };
        }

        protected override AsignaturaResponseDto MapEntityToResponse(Asignatura entity)
        {
            return new AsignaturaResponseDto
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                TipoAsignatura = entity.TipoAsignatura
            };
        }

        protected override void UpdateEntityFromDto(Asignatura entity, AsignaturaDto dto)
        {
            entity.Nombre = dto.Nombre;
            entity.Descripcion = dto.Descripcion;
            entity.TipoAsignatura = dto.TipoAsignatura;
        }
        #endregion

        #region Validaciones
        protected override Task<ApiResponse<AsignaturaResponseDto>> ValidateCreateAsync(AsignaturaDto dto)
        {
            var validationResult = ValidateAsignaturaData(dto);
            if (validationResult != null)
                return Task.FromResult(validationResult);

            return Task.FromResult<ApiResponse<AsignaturaResponseDto>>(null);
        }

        protected override Task<ApiResponse<AsignaturaResponseDto>> ValidateUpdateAsync(int id, AsignaturaDto dto)
        {
            var validationResult = ValidateAsignaturaData(dto);
            if (validationResult != null)
                return Task.FromResult(validationResult);

            return Task.FromResult<ApiResponse<AsignaturaResponseDto>>(null);
        }

        private ApiResponse<AsignaturaResponseDto> ValidateAsignaturaData(AsignaturaDto dto)
        {
            if (dto.Descripcion?.Length > 125)
            {
                return ApiResponse<AsignaturaResponseDto>.ErrorResponse(
                    "La descripción no puede exceder los 125 caracteres"
                );
            }

            var tiposValidos = new[] { "Teorica", "Practica", "TeoricoPractica" };
            if (!tiposValidos.Contains(dto.TipoAsignatura))
            {
                return ApiResponse<AsignaturaResponseDto>.ErrorResponse(
                    "El tipo de asignatura debe ser: Teorica, Practica o TeoricoPractica"
                );
            }

            return null;
        }
        #endregion

        #region Métodos Auxiliares
        private static string GenerarCodigoAsignatura(string nombreAsignatura)
        {
            try
            {
                string limpio = new string(nombreAsignatura
                    .Where(c => char.IsLetter(c))
                    .ToArray());

                if (limpio.Length < 3)
                {
                   
                    limpio = nombreAsignatura.Replace(" ", "");
                    if (limpio.Length < 3)
                        limpio = limpio.PadRight(3, 'X');
                }

   
                string letras = limpio.Substring(0, 3).ToUpper();

                int numero = _random.Next(100, 1000); // 100 hasta 999

                return $"{letras}-{numero}";
            }
            catch (Exception)
            {
                return $"ASG-{_random.Next(100, 1000)}";
            }
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
