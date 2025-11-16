using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class CursoAcademicoService : ICursoAcademicoService
    {
        private readonly ICursoAcademicoRepository _cursoAcademicoRepository;
        private readonly ILogger<CursoAcademicoService> _logger;

        public CursoAcademicoService(ICursoAcademicoRepository cursoAcademicoRepository, ILogger<CursoAcademicoService> logger)
        {
            _cursoAcademicoRepository = cursoAcademicoRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<CursoAcademicoResponseDto>> CreateAsync(CursoAcademicoDto dto)
        {
            try
            {
                var response = new CursoAcademico
                {
                    IdGrado = dto.IdGrado,
                    SchoolYear = dto.SchoolYear
                };

                await _cursoAcademicoRepository.AddAsync(response);

                var cursoAcademicoResponse = new CursoAcademicoResponseDto
                {
                    Id = response.Id,
                    IdGrado = response.IdGrado,
                    Grado = response.Grado,
                    SchoolYear = response.SchoolYear
                };

                return ApiResponse<CursoAcademicoResponseDto>.SuccessResponse(cursoAcademicoResponse, "Curso academico creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el curso académico");
                return ApiResponse<CursoAcademicoResponseDto>.ErrorResponse(
                    "Error al crear el curso Academico",
                    new List<string> { ex.Message }
                    );
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var cursoAcademico = await _cursoAcademicoRepository.GetByIdAsync(id);
                if (cursoAcademico == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Error encontrando el curso");
                }
                await _cursoAcademicoRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true, "Curso eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar el curso",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<CursoAcademicoResponseDto>>> GetAllAsync()
        {
            try
            {
                var cursosAcademicos = await _cursoAcademicoRepository.GetAllAsync();
                _logger.LogInformation($"Cursos Academicos obtenidos: {cursosAcademicos.Count}");

                var cursoAcademicoResponse = new List<CursoAcademicoResponseDto>();

                foreach (var curso in cursosAcademicos)
                {
                    _logger.LogInformation($"Procesando curso: {curso.Id}");

                    cursoAcademicoResponse.Add(new CursoAcademicoResponseDto
                    {
                        Id = curso.Id,
                        IdGrado = curso.IdGrado,
                        Grado = curso.Grado,
                        SchoolYear = curso.SchoolYear
                    });
                }

                _logger.LogInformation("Todos los cursos academicos procesados correctamente");
                return ApiResponse<List<CursoAcademicoResponseDto>>.SuccessResponse(cursoAcademicoResponse, "Cursos obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CursoAcademicoResponseDto>>.ErrorResponse(
                    "Error al obtener los cursos academicos",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CursoAcademicoResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var cursoAcademico = await _cursoAcademicoRepository.GetByIdAsync(id);
                if( cursoAcademico == null)
                {
                    return ApiResponse<CursoAcademicoResponseDto>.ErrorResponse("Curso no encontrado");
                }

                var cursoAcademicoResponse = new CursoAcademicoResponseDto
                {
                    Id = cursoAcademico.Id,
                    IdGrado = cursoAcademico.IdGrado,
                    Grado = cursoAcademico.Grado,
                    SchoolYear = cursoAcademico.SchoolYear
                };

                return ApiResponse<CursoAcademicoResponseDto>.SuccessResponse(cursoAcademicoResponse, "Curso academico obtenido exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<CursoAcademicoResponseDto>.ErrorResponse(
                    "Error al obtener el curso academico",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CursoAcademicoResponseDto>> UpdateAsync(int id, CursoAcademicoDto dto)
        {
            try
            {
                var cursoAcademico = await _cursoAcademicoRepository.GetByIdAsync(id);
                if( cursoAcademico == null)
                {
                    return ApiResponse<CursoAcademicoResponseDto>.ErrorResponse("Curso no encontrado");
                }

                cursoAcademico.IdGrado = dto.IdGrado;
                cursoAcademico.SchoolYear = dto.SchoolYear;

                await _cursoAcademicoRepository.UpdateAsync(cursoAcademico);

                var cursoAcademicoResponse = new CursoAcademicoResponseDto
                {
                    Id = cursoAcademico.Id,
                    IdGrado = cursoAcademico.IdGrado,
                    Grado = cursoAcademico.Grado,
                    SchoolYear = cursoAcademico.SchoolYear
                };

                return ApiResponse<CursoAcademicoResponseDto>.SuccessResponse(cursoAcademicoResponse, "Curso Academico actualizado exitosamentes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el curso académico");
                return ApiResponse<CursoAcademicoResponseDto>.ErrorResponse(
                    "Error al actualizar el curso",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
