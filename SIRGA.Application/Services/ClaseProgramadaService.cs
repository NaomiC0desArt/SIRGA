using Azure;
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
    public class ClaseProgramadaService : BaseService<ClaseProgramada, ClaseProgramadaDto, ClaseProgramadaResponseDto>, IClaseProgramadaService
    {
        private readonly IClaseProgramadaRepository _claseProgramadaRepository;

        public ClaseProgramadaService(
            IClaseProgramadaRepository claseProgramadaRepository,
            ILogger<ClaseProgramadaService> logger)
            : base(claseProgramadaRepository, logger)
        {
            _claseProgramadaRepository = claseProgramadaRepository;
        }

        protected override string EntityName => "Clase Programada";

        #region Mapeos
        protected override ClaseProgramada MapDtoToEntity(ClaseProgramadaDto dto)
        {
            return new ClaseProgramada
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                WeekDay = ConvertirDiaADayOfWeek(dto.WeekDay),
                Location = dto.Location,
                IdAsignatura = dto.IdAsignatura,
                IdProfesor = dto.IdProfesor,
                IdCursoAcademico = dto.IdCursoAcademico
            };
        }

        protected override ClaseProgramadaResponseDto MapEntityToResponse(ClaseProgramada entity)
        {
            // Para el método base que usa GetByIdAsync (entidades simples)
            return new ClaseProgramadaResponseDto
            {
                Id = entity.Id,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                WeekDay = ConvertirDayOfWeekADia(entity.WeekDay),
                Location = entity.Location,
                IdAsignatura = entity.IdAsignatura,
                IdProfesor = entity.IdProfesor,
                IdCursoAcademico = entity.IdCursoAcademico
            };
        }

        protected override void UpdateEntityFromDto(ClaseProgramada entity, ClaseProgramadaDto dto)
        {
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.WeekDay = ConvertirDiaADayOfWeek(dto.WeekDay);
            entity.Location = dto.Location;
            entity.IdAsignatura = dto.IdAsignatura;
            entity.IdProfesor = dto.IdProfesor;
            entity.IdCursoAcademico = dto.IdCursoAcademico;
        }
        #endregion

        #region Validaciones
        protected override async Task<ApiResponse<ClaseProgramadaResponseDto>> ValidateCreateAsync(ClaseProgramadaDto dto)
        {
            return await ValidarClase(dto, null);
        }

        protected override async Task<ApiResponse<ClaseProgramadaResponseDto>> ValidateUpdateAsync(int id, ClaseProgramadaDto dto)
        {
            return await ValidarClase(dto, id);
        }

        private async Task<ApiResponse<ClaseProgramadaResponseDto>> ValidarClase(ClaseProgramadaDto dto, int? excludeClaseId)
        {
            // Validar horarios
            if (dto.EndTime <= dto.StartTime)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                    "La hora de fin debe ser posterior a la hora de inicio"
                );
            }

            var duracion = dto.EndTime - dto.StartTime;
            if (duracion.TotalHours > 2)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                    "Una clase no puede durar más de 2 horas"
                );
            }

            // Validar conflictos de profesor
            var conflicto = await ValidarConflictoProfesor(
                dto.IdProfesor,
                dto.WeekDay,
                dto.StartTime,
                dto.EndTime,
                excludeClaseId
            );

            if (conflicto != null)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                    $"El profesor ya tiene una clase programada el {dto.WeekDay} de {conflicto.StartTime:hh\\:mm} a {conflicto.EndTime:hh\\:mm}"
                );
            }

            return null;
        }

        private async Task<ClaseProgramada> ValidarConflictoProfesor(
            int idProfesor,
            string weekDay,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeClaseId = null)
        {
            var dayOfWeek = ConvertirDiaADayOfWeek(weekDay);
            var todasLasClases = await _claseProgramadaRepository.GetAllAsync();

            return todasLasClases.FirstOrDefault(c =>
                c.IdProfesor == idProfesor &&
                c.WeekDay == dayOfWeek &&
                (!excludeClaseId.HasValue || c.Id != excludeClaseId.Value) &&
                (
                    (startTime >= c.StartTime && startTime < c.EndTime) ||
                    (endTime > c.StartTime && endTime <= c.EndTime) ||
                    (startTime <= c.StartTime && endTime >= c.EndTime)
                )
            );
        }
        #endregion

        #region Hooks Post-Operación
        protected override async Task<ClaseProgramada> AfterCreateAsync(ClaseProgramada entity)
        {
            // Recargar la entidad con todos los detalles
            var claseConDetalles = await _claseProgramadaRepository.GetByIdWithDetailsAsync(entity.Id);
            return ConvertirDetallesAEntidad(claseConDetalles, entity);
        }

        protected override async Task<ClaseProgramada> AfterUpdateAsync(ClaseProgramada entity)
        {
            // Recargar la entidad con todos los detalles
            var claseConDetalles = await _claseProgramadaRepository.GetByIdWithDetailsAsync(entity.Id);
            return ConvertirDetallesAEntidad(claseConDetalles, entity);
        }

        private ClaseProgramada ConvertirDetallesAEntidad(
            Domain.ReadModels.ClaseProgramadaConDetalles detalles,
            ClaseProgramada entityBase)
        {
            // Crear una entidad enriquecida para el mapeo
            var entity = entityBase;

            // Asignar las propiedades de navegación si están disponibles
            // Esto permite que MapEntityToResponse tenga acceso a los datos completos
            if (detalles != null)
            {
                // Aquí podrías cargar las entidades relacionadas si es necesario
                // Por ahora, almacenamos la info en propiedades dinámicas o usamos otro approach
            }

            return entity;
        }
        #endregion

        #region Overrides para manejar detalles
        public override async Task<ApiResponse<ClaseProgramadaResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var claseConDetalles = await _claseProgramadaRepository.GetByIdWithDetailsAsync(id);

                if (claseConDetalles == null)
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse($"{EntityName} no encontrada");

                var response = MapDetallesAResponse(claseConDetalles);
                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(
                    response,
                    $"{EntityName} obtenida exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}");
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                    $"Error al obtener {EntityName}",
                    new List<string> { ex.Message }
                );
            }
        }

        public override async Task<ApiResponse<List<ClaseProgramadaResponseDto>>> GetAllAsync()
        {
            try
            {
                var clasesConDetalles = await _claseProgramadaRepository.GetAllWithDetailsAsync();
                _logger.LogInformation($"{EntityName}s obtenidas: {clasesConDetalles.Count}");

                var responses = clasesConDetalles.Select(MapDetallesAResponse).ToList();

                return ApiResponse<List<ClaseProgramadaResponseDto>>.SuccessResponse(
                    responses,
                    $"{EntityName}s obtenidas exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}s");
                return ApiResponse<List<ClaseProgramadaResponseDto>>.ErrorResponse(
                    $"Error al obtener {EntityName}s",
                    new List<string> { ex.Message }
                );
            }
        }

        private ClaseProgramadaResponseDto MapDetallesAResponse(Domain.ReadModels.ClaseProgramadaConDetalles detalles)
        {
            return new ClaseProgramadaResponseDto
            {
                Id = detalles.Id,
                StartTime = detalles.StartTime,
                EndTime = detalles.EndTime,
                WeekDay = ConvertirDayOfWeekADia(detalles.WeekDay),
                Location = detalles.Location,
                IdAsignatura = detalles.IdAsignatura,
                AsignaturaNombre = detalles.AsignaturaNombre,
                IdProfesor = detalles.IdProfesor,
                ProfesorNombre = $"{detalles.ProfesorFirstName} {detalles.ProfesorLastName}",
                IdCursoAcademico = detalles.IdCursoAcademico,
                CursoAcademicoNombre = $"{detalles.GradoNombre} - Sección {detalles.SeccionNombre} ({detalles.AnioEscolarPeriodo})"
            };
        }
        #endregion

        #region Helpers
        private DayOfWeek ConvertirDiaADayOfWeek(string dia)
        {
            return dia switch
            {
                "Lunes" => DayOfWeek.Monday,
                "Martes" => DayOfWeek.Tuesday,
                "Miércoles" => DayOfWeek.Wednesday,
                "Jueves" => DayOfWeek.Thursday,
                "Viernes" => DayOfWeek.Friday,
                "Sábado" => DayOfWeek.Saturday,
                "Domingo" => DayOfWeek.Sunday,
                _ => throw new ArgumentException($"Día no válido: {dia}")
            };
        }

        private string ConvertirDayOfWeekADia(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => "Desconocido"
            };
        }
        #endregion
    }
}
