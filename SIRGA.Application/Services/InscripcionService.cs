using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Application.Services
{
    public class InscripcionService : IInscripcionService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InscripcionService> _logger;

        public InscripcionService(
            IInscripcionRepository inscripcionRepository,
            ApplicationDbContext context,
            ILogger<InscripcionService> logger)
        {
            _inscripcionRepository = inscripcionRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<InscripcionResponseDto>> CreateAsync(InscripcionDto dto)
        {
            try
            {
                var inscripcion = new Inscripcion
                {
                    IdEstudiante = dto.IdEstudiante,
                    IdCursoAcademico = dto.IdCursoAcademico,
                    FechaInscripcion = dto.FechaInscripcion
                };

                var inscripcionCreada = await _inscripcionRepository.AddAsync(inscripcion);

                // Obtener el estudiante y su usuario
                var estudiante = await _context.Estudiantes
                    .FirstOrDefaultAsync(e => e.Id == inscripcionCreada.IdEstudiante);

                var applicationUser = estudiante != null
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == estudiante.ApplicationUserId)
                    : null;

                var inscripcionResponse = new InscripcionResponseDto
                {
                    Id = inscripcionCreada.Id,
                    IdEstudiante = inscripcionCreada.IdEstudiante,
                    Estudiante = inscripcionCreada.Estudiante,
                    EstudianteNombre = applicationUser != null
                        ? $"{applicationUser.FirstName} {applicationUser.LastName}"
                        : "N/A",
                    EstudianteMatricula = estudiante?.Matricula ?? "N/A",
                    IdCursoAcademico = inscripcionCreada.IdCursoAcademico,
                    CursoAcademico = inscripcionCreada.CursoAcademico,
                    CursoAcademicoNombre = inscripcionCreada.CursoAcademico?.Grado != null
                        ? $"{inscripcionCreada.CursoAcademico.Grado.GradeName} {inscripcionCreada.CursoAcademico.Grado.Section} - {inscripcionCreada.CursoAcademico.SchoolYear}"
                        : "N/A",
                    FechaInscripcion = inscripcionCreada.FechaInscripcion
                };

                return ApiResponse<InscripcionResponseDto>.SuccessResponse(inscripcionResponse, "Inscripción creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la inscripción");
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    "Error al crear la inscripción",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
                if (inscripcion == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Inscripción no encontrada");
                }

                await _inscripcionRepository.DeleteAsync(id);
                return ApiResponse<bool>.SuccessResponse(true, "Inscripción eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la inscripción");
                return ApiResponse<bool>.ErrorResponse(
                    "Error al eliminar la inscripción",
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

                var inscripcionesResponse = new List<InscripcionResponseDto>();

                foreach (var inscrip in inscripciones)
                {
                    _logger.LogInformation($"Procesando inscripción: {inscrip.Id}");

                    // Obtener el estudiante
                    var estudiante = await _context.Estudiantes
                        .FirstOrDefaultAsync(e => e.Id == inscrip.IdEstudiante);

                    // Obtener el ApplicationUser
                    var applicationUser = estudiante != null
                        ? await _context.Users.FirstOrDefaultAsync(u => u.Id == estudiante.ApplicationUserId)
                        : null;

                    // Obtener nombres del estudiante
                    string estudianteNombre = "N/A";
                    string estudianteMatricula = "N/A";

                    if (estudiante != null)
                    {
                        estudianteMatricula = estudiante.Matricula;

                        if (applicationUser != null)
                        {
                            estudianteNombre = $"{applicationUser.FirstName} {applicationUser.LastName}";
                        }
                        else
                        {
                            _logger.LogWarning($"ApplicationUser no encontrado para estudiante {estudiante.Id} con UserId {estudiante.ApplicationUserId}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Estudiante no encontrado para inscripción {inscrip.Id}");
                    }

                    // Obtener nombre del curso académico
                    string cursoNombre = "N/A";
                    if (inscrip.CursoAcademico != null && inscrip.CursoAcademico.Grado != null)
                    {
                        cursoNombre = $"{inscrip.CursoAcademico.Grado.GradeName} {inscrip.CursoAcademico.Grado.Section} - {inscrip.CursoAcademico.SchoolYear}";
                    }
                    else
                    {
                        _logger.LogWarning($"CursoAcademico o Grado nulo para inscripción {inscrip.Id}");
                    }

                    inscripcionesResponse.Add(new InscripcionResponseDto
                    {
                        Id = inscrip.Id,
                        IdEstudiante = inscrip.IdEstudiante,
                        Estudiante = inscrip.Estudiante,
                        EstudianteNombre = estudianteNombre,
                        EstudianteMatricula = estudianteMatricula,
                        IdCursoAcademico = inscrip.IdCursoAcademico,
                        CursoAcademico = inscrip.CursoAcademico,
                        CursoAcademicoNombre = cursoNombre,
                        FechaInscripcion = inscrip.FechaInscripcion
                    });
                }

                _logger.LogInformation("Todas las inscripciones procesadas exitosamente");
                return ApiResponse<List<InscripcionResponseDto>>.SuccessResponse(inscripcionesResponse, "Inscripciones obtenidas exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las inscripciones");
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
                    return ApiResponse<InscripcionResponseDto>.ErrorResponse("Inscripción no encontrada");
                }

                // Obtener el estudiante
                var estudiante = await _context.Estudiantes
                    .FirstOrDefaultAsync(e => e.Id == inscripcion.IdEstudiante);

                // Obtener el ApplicationUser
                var applicationUser = estudiante != null
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == estudiante.ApplicationUserId)
                    : null;

                // Obtener nombres del estudiante
                string estudianteNombre = "N/A";
                string estudianteMatricula = "N/A";

                if (estudiante != null)
                {
                    estudianteMatricula = estudiante.Matricula;

                    if (applicationUser != null)
                    {
                        estudianteNombre = $"{applicationUser.FirstName} {applicationUser.LastName}";
                    }
                }

                // Obtener nombre del curso académico
                string cursoNombre = "N/A";
                if (inscripcion.CursoAcademico != null && inscripcion.CursoAcademico.Grado != null)
                {
                    cursoNombre = $"{inscripcion.CursoAcademico.Grado.GradeName} {inscripcion.CursoAcademico.Grado.Section} - {inscripcion.CursoAcademico.SchoolYear}";
                }

                var inscripcionResponse = new InscripcionResponseDto
                {
                    Id = inscripcion.Id,
                    IdEstudiante = inscripcion.IdEstudiante,
                    Estudiante = inscripcion.Estudiante,
                    EstudianteNombre = estudianteNombre,
                    EstudianteMatricula = estudianteMatricula,
                    IdCursoAcademico = inscripcion.IdCursoAcademico,
                    CursoAcademico = inscripcion.CursoAcademico,
                    CursoAcademicoNombre = cursoNombre,
                    FechaInscripcion = inscripcion.FechaInscripcion
                };

                return ApiResponse<InscripcionResponseDto>.SuccessResponse(inscripcionResponse, "Inscripción obtenida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la inscripción");
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    "Error al obtener la inscripción",
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
                    return ApiResponse<InscripcionResponseDto>.ErrorResponse("Inscripción no encontrada");
                }

                inscripcion.IdEstudiante = dto.IdEstudiante;
                inscripcion.IdCursoAcademico = dto.IdCursoAcademico;
                inscripcion.FechaInscripcion = dto.FechaInscripcion;

                var inscripcionActualizada = await _inscripcionRepository.UpdateAsync(inscripcion);

                
                var estudiante = await _context.Estudiantes
                    .FirstOrDefaultAsync(e => e.Id == inscripcionActualizada.IdEstudiante);

                
                var applicationUser = estudiante != null
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == estudiante.ApplicationUserId)
                    : null;

                
                string estudianteNombre = "N/A";
                string estudianteMatricula = "N/A";

                if (estudiante != null)
                {
                    estudianteMatricula = estudiante.Matricula;

                    if (applicationUser != null)
                    {
                        estudianteNombre = $"{applicationUser.FirstName} {applicationUser.LastName}";
                    }
                }

                
                string cursoNombre = "N/A";
                if (inscripcionActualizada.CursoAcademico != null && inscripcionActualizada.CursoAcademico.Grado != null)
                {
                    cursoNombre = $"{inscripcionActualizada.CursoAcademico.Grado.GradeName} {inscripcionActualizada.CursoAcademico.Grado.Section} - {inscripcionActualizada.CursoAcademico.SchoolYear}";
                }

                var inscripcionResponse = new InscripcionResponseDto
                {
                    Id = inscripcionActualizada.Id,
                    IdEstudiante = inscripcionActualizada.IdEstudiante,
                    Estudiante = inscripcionActualizada.Estudiante,
                    EstudianteNombre = estudianteNombre,
                    EstudianteMatricula = estudianteMatricula,
                    IdCursoAcademico = inscripcionActualizada.IdCursoAcademico,
                    CursoAcademico = inscripcionActualizada.CursoAcademico,
                    CursoAcademicoNombre = cursoNombre,
                    FechaInscripcion = inscripcionActualizada.FechaInscripcion
                };

                return ApiResponse<InscripcionResponseDto>.SuccessResponse(inscripcionResponse, "Inscripción actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la inscripción");
                return ApiResponse<InscripcionResponseDto>.ErrorResponse(
                    "Error al actualizar la inscripción",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
