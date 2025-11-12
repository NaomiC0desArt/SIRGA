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

        public async Task<ApiResponse<ClaseProgramadaResponseDto>> CreateAsync(ClaseProgramadaDto dto)
        {
            try
            {
                var response = new ClaseProgramada
                {
                    Id = dto.Id,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    WeekDay = dto.WeekDay,
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
                    WeekDay = response.WeekDay,
                    Location = response.Location,
                    IdAsignatura = response.IdAsignatura,
                    IdProfesor = response.IdProfesor,
                    IdCursoAcademico = response.IdCursoAcademico
                };
                return ApiResponse<ClaseProgramadaResponseDto>.SuccessResponse(claseProgramadaResponse, "Clase programada creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClaseProgramadaResponseDto>.ErrorResponse("Error al crear la clase programada");
            }
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

                var clasesResponse = new List<ClaseProgramadaResponseDto>();

                foreach (var claseProgramada in clasesProgramadas)
                {
                    _logger.LogInformation($"Procesando Clase: {claseProgramada.Id}");

                    clasesResponse.Add(new ClaseProgramadaResponseDto
                    {
                        Id = claseProgramada.Id,
                        StartTime = claseProgramada.StartTime,
                        EndTime = claseProgramada.EndTime,
                        WeekDay = claseProgramada.WeekDay,
                        Location = claseProgramada.Location,
                        IdAsignatura = claseProgramada.IdAsignatura,
                        IdProfesor = claseProgramada.IdProfesor,
                        IdCursoAcademico = claseProgramada.IdCursoAcademico
                    });
                }

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
                    WeekDay = claseProgramada.WeekDay,
                    Location = claseProgramada.Location,
                    IdAsignatura = claseProgramada.IdAsignatura,
                    IdProfesor = claseProgramada.IdProfesor,
                    IdCursoAcademico = claseProgramada.IdCursoAcademico

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

                claseProgramada.StartTime = dto.StartTime;
                claseProgramada.EndTime = dto.EndTime;
                claseProgramada.WeekDay = dto.WeekDay;
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
                    WeekDay = claseProgramada.WeekDay,
                    Location = claseProgramada.Location,
                    IdAsignatura = claseProgramada.IdAsignatura,
                    IdProfesor = claseProgramada.IdProfesor,
                    IdCursoAcademico = claseProgramada.IdCursoAcademico
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
    }
}
