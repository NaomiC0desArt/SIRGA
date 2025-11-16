using Azure;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class ClaseProgramadaService : IClaseProgramadaService
    {
        private readonly IClaseProgramadaRepository _claseProgramadaRepository;
        private readonly ILogger<ClaseProgramadaService> _logger;

        public ClaseProgramadaService(IClaseProgramadaRepository claseProgramadaRepository, ILogger<ClaseProgramadaService> logger)
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

                var dayOfWeek = ConvertirDiaADayOfWeek(dto.WeekDay);

                var response = new ClaseProgramada
                {
                    Id = dto.Id,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    WeekDay = dayOfWeek,
                    Location = dto.Location,
                    IdAsignatura = dto.IdAsignatura,
                    IdProfesor = dto.IdProfesor,
                    IdCursoAcademico = dto.IdCursoAcademico
                };

                await _claseProgramadaRepository.AddAsync(response);

                var claseProgramadaResponse = new ClaseProgramadaResponseDto
                {
                    Id = response.Id,
                    StartTime = response.StartTime,
                    EndTime = response.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(response.WeekDay),
                    Location = response.Location,
                    IdAsignatura = response.IdAsignatura,
                    Asignatura = response.Asignatura,
                    IdProfesor = response.IdProfesor,
                    ProfesorNombre = response.Profesor != null
                        ? $"{response.Profesor.FirstName} {response.Profesor.LastName}"
                        : null,
                    IdCursoAcademico = response.IdCursoAcademico,
                    CursoAcademicoNombre = response.CursoAcademico?.Grado != null
                        ? $"{response.CursoAcademico.Grado.GradeName} {response.CursoAcademico.Grado.Section}"
                        : null
                };
                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(claseProgramadaResponse, "Clase programada creada exitosamente");
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
                var clasesProgramadas = await _claseProgramadaRepository.GetAllAsync();
                _logger.LogInformation($"Clases programada obtenidas: {clasesProgramadas.Count}");

                var clasesResponse = clasesProgramadas.Select(c => new ClaseProgramadaResponseDto
                {
                    Id = c.Id,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(c.WeekDay),
                    Location = c.Location,
                    IdAsignatura = c.IdAsignatura,
                    AsignaturaNombre = c.Asignatura?.Nombre, // ← MAPEAR NOMBRE
                    IdProfesor = c.IdProfesor,
                    ProfesorNombre = c.Profesor != null // ← MAPEAR NOMBRE
                        ? $"{c.Profesor.FirstName} {c.Profesor.LastName}"
                        : null,
                    IdCursoAcademico = c.IdCursoAcademico,
                    CursoAcademicoNombre = c.CursoAcademico?.Grado != null // ← MAPEAR NOMBRE
                        ? $"{c.CursoAcademico.Grado.GradeName} {c.CursoAcademico.Grado.Section}"
                        : null
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
                var claseProgramada = await _claseProgramadaRepository.GetByIdAsync(id);
                
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
                    AsignaturaNombre = claseProgramada.Asignatura?.Nombre,
                    IdProfesor = claseProgramada.IdProfesor,
                    ProfesorNombre = claseProgramada.Profesor != null
                        ? $"{claseProgramada.Profesor.FirstName} {claseProgramada.Profesor.LastName}"
                        : null,
                    IdCursoAcademico = claseProgramada.IdCursoAcademico,
                    CursoAcademicoNombre = claseProgramada.CursoAcademico?.Grado != null
                        ? $"{claseProgramada.CursoAcademico.Grado.GradeName} {claseProgramada.CursoAcademico.Grado.Section}"
                        : null
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
                var claseProgramada = await _claseProgramadaRepository.GetByIdAsync(id);
                if (claseProgramada == null)
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
                claseProgramada.StartTime = dto.StartTime;
                claseProgramada.EndTime = dto.EndTime;
                claseProgramada.WeekDay = ConvertirDiaADayOfWeek(dto.WeekDay);
                claseProgramada.Location = dto.Location;
                claseProgramada.IdAsignatura = dto.IdAsignatura;
                claseProgramada.IdProfesor = dto.IdProfesor;
                claseProgramada.IdCursoAcademico = dto.IdCursoAcademico;

                await _claseProgramadaRepository.UpdateAsync(claseProgramada);

                var claseProgramadaResponse = new ClaseProgramadaResponseDto
                {
                    Id = claseProgramada.Id,
                    StartTime = claseProgramada.StartTime,
                    EndTime = claseProgramada.EndTime,
                    WeekDay = ConvertirDayOfWeekADia(claseProgramada.WeekDay),
                    Location = claseProgramada.Location,
                    IdAsignatura = claseProgramada.IdAsignatura,
                    AsignaturaNombre = claseProgramada.Asignatura?.Nombre,
                    IdProfesor = claseProgramada.IdProfesor,
                    ProfesorNombre = claseProgramada.Profesor != null
                        ? $"{claseProgramada.Profesor.FirstName} {claseProgramada.Profesor.LastName}"
                        : null,
                    IdCursoAcademico = claseProgramada.IdCursoAcademico,
                    CursoAcademicoNombre = claseProgramada.CursoAcademico?.Grado != null
                        ? $"{claseProgramada.CursoAcademico.Grado.GradeName} {claseProgramada.CursoAcademico.Grado.Section}"
                        : null
                };

                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(claseProgramadaResponse, "Clase programada actualizada exitosamente");
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
