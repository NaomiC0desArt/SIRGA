using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Application.Services
{
    public class InscripcionService : BaseService<Inscripcion, InscripcionDto, InscripcionResponseDto>, IInscripcionService
    {
        private readonly IInscripcionRepository _inscripcionRepository;

        public InscripcionService(
            IInscripcionRepository inscripcionRepository,
            ILogger<InscripcionService> logger)
            : base(inscripcionRepository, logger)
        {
            _inscripcionRepository = inscripcionRepository;
        }

        protected override string EntityName => "Inscripción";

        #region Mapeos
        protected override Inscripcion MapDtoToEntity(InscripcionDto dto)
        {
            return new Inscripcion
            {
                IdEstudiante = dto.IdEstudiante,
                IdCursoAcademico = dto.IdCursoAcademico,
                FechaInscripcion = dto.FechaInscripcion
            };
        }

        protected override InscripcionResponseDto MapEntityToResponse(Inscripcion entity)
        {
            // Para el caso base (cuando solo tenemos la entidad)
            return new InscripcionResponseDto
            {
                Id = entity.Id,
                IdEstudiante = entity.IdEstudiante,
                IdCursoAcademico = entity.IdCursoAcademico,
                FechaInscripcion = entity.FechaInscripcion,
                Estudiante = entity.Estudiante,
                CursoAcademico = entity.CursoAcademico
            };
        }

        protected override void UpdateEntityFromDto(Inscripcion entity, InscripcionDto dto)
        {
            entity.IdEstudiante = dto.IdEstudiante;
            entity.IdCursoAcademico = dto.IdCursoAcademico;
            entity.FechaInscripcion = dto.FechaInscripcion;
        }
        #endregion

        #region Overrides para manejar detalles
        public override async Task<ApiResponse<InscripcionResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var inscripcionConDetalles = await _inscripcionRepository.GetInscripcionConDetallesAsync(id);

                if (inscripcionConDetalles == null)
                    return ApiResponse<InscripcionResponseDto>.ErrorResponse($"{EntityName} no encontrada");

                var response = MapDetallesAResponse(inscripcionConDetalles);
                return ApiResponse<InscripcionResponseDto>.SuccessResponse(
                    response,
                    $"{EntityName} obtenida exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}");
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    $"Error al obtener {EntityName}",
                    new List<string> { ex.Message }
                );
            }
        }

        public override async Task<ApiResponse<List<InscripcionResponseDto>>> GetAllAsync()
        {
            try
            {
                var inscripcionesConDetalles = await _inscripcionRepository.GetAllInscripcionesConDetallesAsync();
                _logger.LogInformation($"{EntityName}s obtenidas: {inscripcionesConDetalles.Count}");

                var responses = inscripcionesConDetalles.Select(MapDetallesAResponse).ToList();

                return ApiResponse<List<InscripcionResponseDto>>.SuccessResponse(
                    responses,
                    $"{EntityName}s obtenidas exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}s");
                return ApiResponse<List<InscripcionResponseDto>>.ErrorResponse(
                    $"Error al obtener {EntityName}s",
                    new List<string> { ex.Message }
                );
            }
        }
        #endregion

        #region Mapeo de detalles
        private InscripcionResponseDto MapDetallesAResponse(Domain.ReadModels.InscripcionConDetalles detalles)
        {
            return new InscripcionResponseDto
            {
                Id = detalles.Id,
                IdEstudiante = detalles.IdEstudiante,
                IdCursoAcademico = detalles.IdCursoAcademico,
                FechaInscripcion = detalles.FechaInscripcion,
                EstudianteNombre = detalles.EstudianteNombreCompleto,
                EstudianteMatricula = detalles.EstudianteMatricula,
                CursoAcademicoNombre = detalles.CursoNombre
            };
        }
        #endregion
    }
}
