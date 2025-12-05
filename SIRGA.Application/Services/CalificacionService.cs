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
    public class CalificacionService(ICalificacionRepository calificacionRepository, ILogger<CalificacionService> logger, ApplicationDbContext db)
        : ICalificacionService
    {
        private readonly ICalificacionRepository _calificacionRepository = calificacionRepository;
        private readonly ILogger<CalificacionService> _logger = logger;
        private readonly ApplicationDbContext _db = db;

        public async Task<ApiResponse<CalificacionResponseDto>> CreateAsync(CalificacionDto dto)
        {
            try
            {
                var response = new Calificacion
                {
                    Id = dto.Id,
                    EstudianteId = dto.EstudianteId,
                    AsignaturaId = dto.AsignaturaId,
                    CursoAcademicoId = dto.CursoAcademicoId,
                    PeriodoId = dto.PeriodoId
                };
                switch (response.Asignatura.TipoAsignatura)
                {
                    case "Teorica":
                        response.Tareas = dto.Tareas; // 40pts
                        response.ExamenesTeoricos = dto.ExamenesTeoricos; // 25pts
                        response.Exposiciones = dto.Exposiciones; // 20pts
                        response.Participacion = dto.Participacion; // 15pts

                        response.NotaPeriodo = (dto.Tareas + dto.ExamenesTeoricos +
                            dto.Exposiciones + dto.Participacion);
                        break;

                    case "Practica":
                        response.Practicas = dto.Practicas; // 50pts
                        response.ProyectoFinal = dto.ProyectoFinal; // 30pts
                        response.Teoria = dto.Teoria; // 10pts
                        response.Participacion = dto.Participacion; // 10pts

                        response.NotaPeriodo = (dto.Practicas + dto.ProyectoFinal + dto.Teoria + dto.Participacion);
                        break;

                    case "TeoricoPractica":
                        response.Examenes = dto.Examenes; // 30pts
                        response.Practicas = dto.Practicas; // 40pts
                        response.Proyectos = dto.Proyectos; // 20pts
                        response.Participacion = dto.Participacion; // 10pts

                        response.NotaPeriodo = (dto.Examenes + dto.Practicas + dto.Proyectos +  dto.Participacion);
                        break;

                    default:
                        return ApiResponse<CalificacionResponseDto>.ErrorResponse("Tipo de Asignatura no existente");
                }

               await _calificacionRepository.AddAsync(response);

                var calificacionResponse = new CalificacionResponseDto
                {
                    Id = response.Id,
                    EstudianteId = response.EstudianteId,
                    AsignaturaId = response.AsignaturaId,
                    CursoAcademicoId = response.CursoAcademicoId,
                    PeriodoId = response.PeriodoId,
                    
                    Tareas = response.Tareas,
                    ExamenesTeoricos = response.ExamenesTeoricos,
                    Exposiciones = response.Exposiciones,
                    Participacion = response.Participacion,
                    Practicas = response.Practicas,
                    ProyectoFinal = response.ProyectoFinal,
                    Teoria = response.Teoria,
                    Examenes = response.Examenes,
                    Proyectos = response.Proyectos,
                    
                    NotaPeriodo = response.NotaPeriodo,
                    Publicado = response.Publicado
                };

                return ApiResponse<CalificacionResponseDto>.SuccessResponse(calificacionResponse);

            }
            catch (Exception ex)
            {
                return ApiResponse<CalificacionResponseDto>.ErrorResponse("Error creando la calificación");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var calificacion = await _calificacionRepository.GetByIdAsync(id);
                if (calificacion == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Calificacion no encontrada");
                }

                await _calificacionRepository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Error al intentar eliminar la calificaion",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<CalificacionResponseDto>>> GetAllAsync()
        {
            try
            {
                var calificacion = await _calificacionRepository.GetAllAsync();
                _logger.LogInformation($"Cantidad de calificaciones obtenidas: {calificacion.Count}");

                var calificacionResponse = new List<CalificacionResponseDto>();

                foreach (var item in calificacion)
                {
                    calificacionResponse.Add(new CalificacionResponseDto
                    {
                        Id = item.Id,
                        EstudianteId = item.EstudianteId,
                        AsignaturaId = item.AsignaturaId,
                        AsignaturaNombre = item.Asignatura.Nombre,
                        CursoAcademicoId = item.CursoAcademicoId,
                        PeriodoId = item.PeriodoId,
                        PeriodoNumero = item.Periodo.Numero,
                        Tareas = item.Tareas,
                        ExamenesTeoricos = item.ExamenesTeoricos,
                        Exposiciones = item.Exposiciones,
                        Participacion = item.Participacion,
                        Practicas = item.Practicas,
                        ProyectoFinal = item.ProyectoFinal,
                        Teoria = item.Teoria,
                        Examenes = item.Examenes,
                        Proyectos = item.Proyectos,
                        NotaPeriodo = item.NotaPeriodo,
                        Publicado = item.Publicado
                    });
                }

                _logger.LogInformation($"Cantidad de calificaciones mapeadas: {calificacionResponse.Count}");
                return ApiResponse<List<CalificacionResponseDto>>.SuccessResponse(calificacionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las calificaciones");
                return ApiResponse<List<CalificacionResponseDto>>.ErrorResponse(
                    "Error al obtener las calificaciones",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CalificacionResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var calificacion = await  _calificacionRepository.GetByIdAsync(id);
                if (calificacion == null)
                {
                    return ApiResponse<CalificacionResponseDto>.ErrorResponse("Calificacion no encontrada");
                }
                var calificacionDto = new CalificacionResponseDto
                {
                    Id = calificacion.Id,
                    EstudianteId = calificacion.EstudianteId,
                    AsignaturaId = calificacion.AsignaturaId,
                    CursoAcademicoId = calificacion.CursoAcademicoId,
                    PeriodoId = calificacion.PeriodoId,
                    Tareas = calificacion.Tareas,
                    ExamenesTeoricos = calificacion.ExamenesTeoricos,
                    Exposiciones = calificacion.Exposiciones,
                    Participacion = calificacion.Participacion,
                    Practicas = calificacion.Practicas,
                    ProyectoFinal = calificacion.ProyectoFinal,
                    Teoria = calificacion.Teoria,
                    Examenes = calificacion.Examenes,
                    Proyectos = calificacion.Proyectos,
                    NotaPeriodo = calificacion.NotaPeriodo,
                    Publicado = calificacion.Publicado
                };
                return ApiResponse<CalificacionResponseDto>.SuccessResponse(calificacionDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<CalificacionResponseDto>.ErrorResponse(
                    "Error al obtener la Calificacion",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CalificacionResponseDto>> PublicarCalificacionAsync(int Id)
        {
            try
            {
                var calificacionUpdate = await _calificacionRepository.GetByIdAsync(Id);

                calificacionUpdate.Publicado = true;

                var publicacion = await _calificacionRepository.UpdateAsync(calificacionUpdate);

                var publicacionDto = new CalificacionResponseDto
                {
                    Id = publicacion.Id,
                    EstudianteId = publicacion.EstudianteId,
                    AsignaturaId = publicacion.AsignaturaId,
                    CursoAcademicoId = publicacion.CursoAcademicoId,
                    PeriodoId = publicacion.PeriodoId,
                    Tareas = publicacion.Tareas,
                    ExamenesTeoricos = publicacion.ExamenesTeoricos,
                    Exposiciones = publicacion.Exposiciones,
                    Participacion = publicacion.Participacion,
                    Practicas = publicacion.Practicas,
                    ProyectoFinal = publicacion.ProyectoFinal,
                    Teoria = publicacion.Teoria,
                    Examenes = publicacion.Examenes,
                    Proyectos = publicacion.Proyectos,
                    NotaPeriodo = publicacion.NotaPeriodo,
                    Publicado = publicacion.Publicado
                };

                return ApiResponse<CalificacionResponseDto>.SuccessResponse(publicacionDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<CalificacionResponseDto>.ErrorResponse(
                    "Error al actualizar la Calificacion",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CalificacionResponseDto>> UpdateAsync(int id, CalificacionDto dto)
        {
            try
            {
                var calificacion = await _calificacionRepository.GetByIdAsync(id);
                if (calificacion == null)
                {
                    return ApiResponse<CalificacionResponseDto>.ErrorResponse("Calificacion no encontrada");
                }
                calificacion.EstudianteId = dto.EstudianteId;
                calificacion.AsignaturaId = dto.AsignaturaId;
                calificacion.CursoAcademicoId = dto.CursoAcademicoId;
                calificacion.PeriodoId = dto.PeriodoId;
                calificacion.Tareas = dto.Tareas;
                calificacion.ExamenesTeoricos = dto.ExamenesTeoricos;
                calificacion.Exposiciones = dto.Exposiciones;
                calificacion.Participacion = dto.Participacion;
                calificacion.Practicas = dto.Practicas;
                calificacion.ProyectoFinal = dto.ProyectoFinal;
                calificacion.Teoria = dto.Teoria;
                calificacion.Examenes = dto.Examenes;
                calificacion.Proyectos = dto.Proyectos;
                calificacion.NotaPeriodo = dto.NotaPeriodo;
                calificacion.Publicado = dto.Publicado;

                var updatedCalificacion = await _calificacionRepository.UpdateAsync(calificacion);

                var calificacionResponse = new CalificacionResponseDto
                {
                    Id = updatedCalificacion.Id,
                    EstudianteId = updatedCalificacion.EstudianteId,
                    AsignaturaId = updatedCalificacion.AsignaturaId,
                    CursoAcademicoId = updatedCalificacion.CursoAcademicoId,
                    PeriodoId = updatedCalificacion.PeriodoId,
                    Tareas = updatedCalificacion.Tareas,
                    ExamenesTeoricos = updatedCalificacion.ExamenesTeoricos,
                    Exposiciones = updatedCalificacion.Exposiciones,
                    Participacion = updatedCalificacion.Participacion,
                    Practicas = updatedCalificacion.Practicas,
                    ProyectoFinal = updatedCalificacion.ProyectoFinal,
                    Teoria = updatedCalificacion.Teoria,
                    Examenes = updatedCalificacion.Examenes,
                    Proyectos = updatedCalificacion.Proyectos,
                    NotaPeriodo = updatedCalificacion.NotaPeriodo,
                    Publicado = updatedCalificacion.Publicado
                };

                return ApiResponse<CalificacionResponseDto>.SuccessResponse(calificacionResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<CalificacionResponseDto>.ErrorResponse(
                    "Error al actualizar la Calificacion",
                    new List<string> { ex.Message }
                );
            }
        }

        // Devuelve P1..P4 y Total para un estudiante/asignatura/curso/año
        public async Task<ApiResponse<AnnualGradeDto>> GetAnnualGradesAsync(int estudianteId, int asignaturaId, int cursoId, int anioEscolarId)
        {
            // Obtener los periodos del año escolar (idealmente 1..4)
            var periodos = await _db.Periodos
                             .Where(p => p.AnioEscolarId == anioEscolarId)
                             .OrderBy(p => p.Numero)
                             .ToListAsync();

            decimal[] p = new decimal[4] { 0, 0, 0, 0 };

            foreach (var per in periodos)
            {
                var cal = await _db.Calificaciones
                            .Where(c => c.EstudianteId == estudianteId
                                     && c.AsignaturaId == asignaturaId
                                     && c.CursoAcademicoId == cursoId
                                     && c.PeriodoId == per.Id
                                     && c.Publicado == true) // solo publicadas
                            .Select(c => c.NotaPeriodo)
                            .FirstOrDefaultAsync();

                // nota no encontrada => 0 (como acordado)
                int idx = (int)(per.Numero - 1);
                if (idx >= 0 && idx < 4)
                    p[idx] = (decimal)cal; // cal ya es 0 si no existe
            }

            decimal total = (p[0] + p[1] + p[2] + p[3]) / 4m;
            total = Math.Round(total, 2);

            var annualGrate = new AnnualGradeDto
            {
                P1 = p[0],
                P2 = p[1],
                P3 = p[2],
                P4 = p[3],
                Total = total
            };
            return ApiResponse<AnnualGradeDto>.SuccessResponse(annualGrate);
        }
    }
}
