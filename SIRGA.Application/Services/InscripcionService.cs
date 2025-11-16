using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class InscripcionService : IInscripcionService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly ILogger<InscripcionService> _logger;

        public InscripcionService(IInscripcionRepository inscripcionRepository, ILogger<InscripcionService> logger)
        {
            _inscripcionRepository = inscripcionRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<InscripcionResponseDto>> CreateAsync(InscripcionDto dto)
        {
            try
            {
                var response = new Inscripcion
                {
                    Id = dto.Id,
                    IdEstudiante = dto.IdEstudiante,
                    IdCursoAcademico = dto.IdCursoAcademico,
                    FechaInscripcion = dto.FechaInscripcion
                };

                await _inscripcionRepository.AddAsync(response);

                var inscripcionResponse = new InscripcionResponseDto
                {
                    Id = response.Id,
                    IdEstudiante = response.IdEstudiante,
                    Estudiante = response.Estudiante,
                    IdCursoAcademico = response.IdCursoAcademico,
                    CursoAcademico = response.CursoAcademico,
                    FechaInscripcion = response.FechaInscripcion
                };

                return ApiResponse<InscripcionResponseDto>.SuccessResponse(inscripcionResponse, "Inscripcion creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    "Error al crear la inscripcion",
                    new List<string> { ex.Message }
                    );
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
                if(inscripcion == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Error obteniendo la Inscripcion");
                }
                await _inscripcionRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true, "Inscripcion eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la Inscripcion",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<InscripcionResponseDto>>> GetAllAsync()
        {
            try
            {
                var inscripciones = await _inscripcionRepository.GetAllAsync();
                _logger.LogInformation($"Inscripciones obtenidas: {inscripciones.Count}");

                var inscripcionesResponse  = new List<InscripcionResponseDto>();

                foreach(var inscrip in inscripciones)
                {
                    _logger.LogInformation($"Procesando inscripcion: {inscrip.Id}");

                    inscripcionesResponse.Add(new InscripcionResponseDto
                    {
                        Id = inscrip.Id,
                        IdEstudiante = inscrip.IdEstudiante,
                        Estudiante = inscrip.Estudiante,
                        IdCursoAcademico = inscrip.IdCursoAcademico,
                        CursoAcademico = inscrip.CursoAcademico,
                        FechaInscripcion = inscrip.FechaInscripcion
                    });
                }

                _logger.LogInformation("Todas las inscripciones procesadas exitosamente");
                return ApiResponse<List<InscripcionResponseDto>>.SuccessResponse(inscripcionesResponse, "Inscripciones Obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<InscripcionResponseDto>>.ErrorResponse(
                    "Error al obtener las inscripciones",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<InscripcionResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
                if (inscripcion == null)
                {
                    return ApiResponse<InscripcionResponseDto>.ErrorResponse("Error obteniendo la Inscripcion");
                }

                var inscripcionResponse = new InscripcionResponseDto
                {
                    Id = inscripcion.Id,
                    IdEstudiante = inscripcion.IdEstudiante,
                    Estudiante = inscripcion.Estudiante,
                    IdCursoAcademico = inscripcion.IdCursoAcademico,
                    CursoAcademico = inscripcion.CursoAcademico,
                    FechaInscripcion = inscripcion.FechaInscripcion
                };

                return ApiResponse<InscripcionResponseDto>.SuccessResponse(inscripcionResponse, "Inscripcion obtenido exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    "Error al obtener la inscripcion",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<InscripcionResponseDto>> UpdateAsync(int id, InscripcionDto dto)
        {
            try
            {
                var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
                if (inscripcion == null)
                {
                    return ApiResponse<InscripcionResponseDto>.ErrorResponse("Error obteniendo la Inscripcion");
                }

                inscripcion.IdEstudiante = dto.IdEstudiante;
                inscripcion.IdCursoAcademico = dto.IdCursoAcademico;
                inscripcion.FechaInscripcion = dto.FechaInscripcion;

                await _inscripcionRepository.UpdateAsync(inscripcion);

                var inscripcionResponse = new InscripcionResponseDto
                {
                    Id = inscripcion.Id,
                    IdEstudiante = inscripcion.IdEstudiante,
                    Estudiante = inscripcion.Estudiante,
                    IdCursoAcademico = inscripcion.IdCursoAcademico,
                    CursoAcademico = inscripcion.CursoAcademico,
                    FechaInscripcion = inscripcion.FechaInscripcion
                };

                return ApiResponse<InscripcionResponseDto>.SuccessResponse(inscripcionResponse, "Inscripcion actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    "Error al actualizar la inscripcion",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
