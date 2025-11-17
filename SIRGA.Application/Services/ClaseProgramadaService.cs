using Azure;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.Interfaces;

namespace SIRGA.Application.Services
{
    public class ClaseProgramadaService : IClaseProgramadaService
    {
        private readonly IClaseProgramadaRepositoryExtended _claseProgramadaRepository;
        private readonly ILogger<ClaseProgramadaService> _logger;

        public ClaseProgramadaService(IClaseProgramadaRepositoryExtended claseProgramadaRepository, ILogger<ClaseProgramadaService> logger)
        {
            _claseProgramadaRepository = claseProgramadaRepository;
            _logger = logger;
        }
        #region Create
        public async Task<ApiResponse<ClaseProgramadaResponseDto>> CreateAsync(ClaseProgramadaDto dto)
        {
            try
            {
                if (dto.EndTime <= dto.StartTime)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        "La hora de fin debe ser posterior a la hora de inicio");
                }

                var duracion = dto.EndTime - dto.StartTime;
                if (duracion.TotalHours > 2)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        "Una clase no puede durar más de 2 horas");
                }

                var conflicto = await ValidarConflictoProfesor(
                    dto.IdProfesor,
                    dto.WeekDay,
                    dto.StartTime,
                    dto.EndTime);

                if (conflicto != null)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        $"El profesor ya tiene una clase programada el {dto.WeekDay} de {conflicto.StartTime:hh\\:mm} a {conflicto.EndTime:hh\\:mm}");
                }

                var nuevaClase = new ClaseProgramada
                {
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    WeekDay = ConvertirDiaADayOfWeek(dto.WeekDay),
                    Location = dto.Location,
                    IdAsignatura = dto.IdAsignatura,
                    IdProfesor = dto.IdProfesor,
                    IdCursoAcademico = dto.IdCursoAcademico
                };

                await _claseProgramadaRepository.AddAsync(nuevaClase);

                // ✅ Obtener la clase con detalles después de crearla
                var claseConDetalles = await _claseProgramadaRepository.GetByIdWithDetailsAsync(nuevaClase.Id);

                if (claseConDetalles == null)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        "Error al obtener los detalles de la clase creada");
                }

                var response = new ClaseProgramadaResponseDto
                {
                    Id = claseConDetalles.Id,
                    StartTime = claseConDetalles.StartTime,
                    EndTime = claseConDetalles.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(claseConDetalles.WeekDay),
                    Location = claseConDetalles.Location,
                    IdAsignatura = claseConDetalles.IdAsignatura,
                    AsignaturaNombre = claseConDetalles.AsignaturaNombre,
                    IdProfesor = claseConDetalles.IdProfesor,
                    ProfesorNombre = $"{claseConDetalles.ProfesorFirstName} {claseConDetalles.ProfesorLastName}",
                    IdCursoAcademico = claseConDetalles.IdCursoAcademico,
                    CursoAcademicoNombre = $"{claseConDetalles.GradoNombre} {claseConDetalles.GradoSeccion}"
                };
                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(response, "Clase programada creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse("Error al crear la clase programada");
            }
        }
        #endregion
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

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var claseProgramada = await _claseProgramadaRepository.GetByIdAsync(id);
                if(claseProgramada == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Error Encontrando la clase");
                }
                await _claseProgramadaRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true, "Clase Programada eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la clase programada",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<ClaseProgramadaResponseDto>>> GetAllAsync()
        {
            try
            {
                var clasesProgramadas = await _claseProgramadaRepository.GetAllWithDetailsAsync();
                _logger.LogInformation($"Clases programada obtenidas: {clasesProgramadas.Count}");

                var clasesResponse = clasesProgramadas.Select(c => new ClaseProgramadaResponseDto
                {
                    Id = c.Id,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(c.WeekDay),
                    Location = c.Location,
                    IdAsignatura = c.IdAsignatura,
                    AsignaturaNombre = c.AsignaturaNombre,
                    IdProfesor = c.IdProfesor,
                    ProfesorNombre = $"{c.ProfesorFirstName} {c.ProfesorLastName}", 
                    IdCursoAcademico = c.IdCursoAcademico,
                    CursoAcademicoNombre = $"{c.GradoNombre} {c.GradoSeccion}"
                }).ToList();


                _logger.LogInformation("Todas las clases programadas procesadas correctamente");
                return ApiResponse<List<ClaseProgramadaResponseDto>>.SuccessResponse(clasesResponse, "Clases obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ClaseProgramadaResponseDto>>.ErrorResponse(
                    "Error al obtener las clases programadas",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClaseProgramadaResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var claseProgramada = await _claseProgramadaRepository.GetByIdWithDetailsAsync(id);

                if (claseProgramada == null)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse("Clase no encontrada");
                }

                var claseProgramadaResponse = new ClaseProgramadaResponseDto
                {
                    Id = claseProgramada.Id,
                    StartTime = claseProgramada.StartTime,
                    EndTime = claseProgramada.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(claseProgramada.WeekDay),
                    Location = claseProgramada.Location,
                    IdAsignatura = claseProgramada.IdAsignatura,
                    AsignaturaNombre = claseProgramada.AsignaturaNombre,
                    IdProfesor = claseProgramada.IdProfesor,
                    ProfesorNombre = $"{claseProgramada.ProfesorFirstName} {claseProgramada.ProfesorLastName}",
                    IdCursoAcademico = claseProgramada.IdCursoAcademico,
                    CursoAcademicoNombre = $"{claseProgramada.GradoNombre} {claseProgramada.GradoSeccion}"
                };

                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(claseProgramadaResponse, "Clase programada obtenida exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                    "Error al obtener la clase programada",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClaseProgramadaResponseDto>> UpdateAsync(int id, ClaseProgramadaDto dto)
        {
            try
            {
                var claseExistente = await _claseProgramadaRepository.GetByIdAsync(id);
                if (claseExistente == null)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse("Clase no encontrada");
                }

                if (dto.EndTime <= dto.StartTime)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        "La hora de fin debe ser posterior a la hora de inicio");
                }

                var duracion = dto.EndTime - dto.StartTime;
                if (duracion.TotalHours > 2)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        "Una clase no puede durar más de 2 horas");
                }

                var conflicto = await ValidarConflictoProfesor(
                    dto.IdProfesor,
                    dto.WeekDay,
                    dto.StartTime,
                    dto.EndTime,
                    id);

                if (conflicto != null)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        $"El profesor ya tiene una clase programada el {dto.WeekDay} de {conflicto.StartTime:hh\\:mm} a {conflicto.EndTime:hh\\:mm}");
                }
                claseExistente.StartTime = dto.StartTime;
                claseExistente.EndTime = dto.EndTime;
                claseExistente.WeekDay = ConvertirDiaADayOfWeek(dto.WeekDay);
                claseExistente.Location = dto.Location;
                claseExistente.IdAsignatura = dto.IdAsignatura;
                claseExistente.IdProfesor = dto.IdProfesor;
                claseExistente.IdCursoAcademico = dto.IdCursoAcademico;

                await _claseProgramadaRepository.UpdateAsync(claseExistente);

                var claseConDetalles = await _claseProgramadaRepository.GetByIdWithDetailsAsync(id);

                if (claseConDetalles == null)
                {
                    return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                        "Error al obtener los detalles de la clase actualizada");
                }

                var response = new ClaseProgramadaResponseDto
                {
                    Id = claseConDetalles.Id,
                    StartTime = claseConDetalles.StartTime,
                    EndTime = claseConDetalles.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(claseConDetalles.WeekDay),
                    Location = claseConDetalles.Location,
                    IdAsignatura = claseConDetalles.IdAsignatura,
                    AsignaturaNombre = claseConDetalles.AsignaturaNombre,
                    IdProfesor = claseConDetalles.IdProfesor,
                    ProfesorNombre = $"{claseConDetalles.ProfesorFirstName} {claseConDetalles.ProfesorLastName}",
                    IdCursoAcademico = claseConDetalles.IdCursoAcademico,
                    CursoAcademicoNombre = $"{claseConDetalles.GradoNombre} {claseConDetalles.GradoSeccion}"
                };


                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(response, "Clase programada actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse(
                    "Error al actualizar la clase",
                    new List<string> { ex.Message }
                );
            }
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
                (!excludeClaseId.HasValue || c.Id != excludeClaseId.Value) && // Excluir clase actual en UPDATE
                (
                    // Caso 1: Nueva clase empieza durante una clase existente
                    (startTime >= c.StartTime && startTime < c.EndTime) ||
                    // Caso 2: Nueva clase termina durante una clase existente
                    (endTime > c.StartTime && endTime <= c.EndTime) ||
                    // Caso 3: Nueva clase engloba completamente una clase existente
                    (startTime <= c.StartTime && endTime >= c.EndTime)
                )
            );
        }
    }
}
