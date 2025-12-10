using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Estudiante;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
namespace SIRGA.Application.Services
{
    public class HorarioEstudianteService :  IHorarioEstudianteService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IClaseProgramadaRepository _claseProgramadaRepository;
        private readonly ILogger<HorarioEstudianteService> _logger;

        public HorarioEstudianteService(
            IInscripcionRepository inscripcionRepository,
            IClaseProgramadaRepository claseProgramadaRepository,
            ILogger<HorarioEstudianteService> logger)
        {
            _inscripcionRepository = inscripcionRepository;
            _claseProgramadaRepository = claseProgramadaRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<HorarioSemanalDto>> GetHorarioByEstudianteIdAsync(int estudianteId)
        {
            try
            {
                // 1. Obtener inscripción activa del estudiante
                var inscripcion = await _inscripcionRepository.GetByConditionAsync(
                    i => i.IdEstudiante == estudianteId
                );

                if (inscripcion == null)
                {
                    return ApiResponse<HorarioSemanalDto>.ErrorResponse(
                        "El estudiante no tiene una inscripción activa"
                    );
                }

                // 2. Obtener todas las clases del curso académico con detalles
                var clasesProgramadas = await _claseProgramadaRepository
                    .GetClasesPorCursoAcademicoAsync(inscripcion.IdCursoAcademico);

                if (!clasesProgramadas.Any())
                {
                    return ApiResponse<HorarioSemanalDto>.SuccessResponse(
                        new HorarioSemanalDto
                        {
                            HorarioSemanal = new List<HorarioEstudianteDto>(),
                            // CORREGIDO: usar AnioEscolar.Periodo
                            PeriodoAcademico = inscripcion.CursoAcademico?.AnioEscolar?.Periodo ?? "N/A",
                            // CORREGIDO: usar Seccion.Nombre
                            GradoSeccion = inscripcion.CursoAcademico != null
                                ? $"{inscripcion.CursoAcademico.Grado?.GradeName} - Sección {inscripcion.CursoAcademico.Seccion?.Nombre}"
                                : "N/A"
                        },
                        "No hay clases programadas para este curso"
                    );
                }

                // 3. Organizar por día de la semana
                var diasSemana = new[]
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                };

                var horarioSemanal = new List<HorarioEstudianteDto>();
                var ahora = DateTime.Now.TimeOfDay;
                var diaActual = DateTime.Now.DayOfWeek;

                foreach (var dia in diasSemana)
                {
                    var clasesDelDia = clasesProgramadas
                        .Where(c => c.WeekDay == dia)
                        .OrderBy(c => c.StartTime)
                        .Select(c =>
                        {
                            var esClaseActual = dia == diaActual &&
                                               ahora >= c.StartTime &&
                                               ahora <= c.EndTime;

                            var esProximaClase = false;
                            if (dia == diaActual && !esClaseActual)
                            {
                                var clasesRestantes = clasesProgramadas
                                    .Where(cl => cl.WeekDay == dia && cl.StartTime > ahora)
                                    .OrderBy(cl => cl.StartTime)
                                    .FirstOrDefault();

                                esProximaClase = clasesRestantes?.Id == c.Id;
                            }

                            return new ClaseHorarioDto
                            {
                                IdClaseProgramada = c.Id,
                                HoraInicio = c.StartTime,
                                HoraFin = c.EndTime,
                                NombreAsignatura = c.AsignaturaNombre,
                                NombreProfesor = c.ProfesorNombreCompleto,
                                Ubicacion = c.Location ?? "N/A",
                                EsClaseActual = esClaseActual,
                                EsProximaClase = esProximaClase
                            };
                        })
                        .ToList();

                    horarioSemanal.Add(new HorarioEstudianteDto
                    {
                        DiaSemana = ConvertirDayOfWeekAEspanol(dia),
                        Clases = clasesDelDia
                    });
                }

                var resultado = new HorarioSemanalDto
                {
                    HorarioSemanal = horarioSemanal,
                    // CORREGIDO: usar AnioEscolar.Periodo
                    PeriodoAcademico = inscripcion.CursoAcademico?.AnioEscolar?.Periodo ?? "N/A",
                    // CORREGIDO: usar Seccion.Nombre
                    GradoSeccion = inscripcion.CursoAcademico != null
                        ? $"{inscripcion.CursoAcademico.Grado?.GradeName} - Sección {inscripcion.CursoAcademico.Seccion?.Nombre}"
                        : "N/A"
                };

                return ApiResponse<HorarioSemanalDto>.SuccessResponse(
                    resultado,
                    "Horario obtenido exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener horario del estudiante {estudianteId}");
                return ApiResponse<HorarioSemanalDto>.ErrorResponse(
                    "Error al obtener el horario",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<ClaseHorarioDto>>> GetClasesDelDiaAsync(
            int estudianteId,
            DayOfWeek diaSemana)
        {
            try
            {
                // 1. Obtener inscripción activa del estudiante
                var inscripcion = await _inscripcionRepository.GetByConditionAsync(
                    i => i.IdEstudiante == estudianteId
                );

                if (inscripcion == null)
                {
                    return ApiResponse<List<ClaseHorarioDto>>.ErrorResponse(
                        "El estudiante no tiene una inscripción activa"
                    );
                }

                // 2. Obtener clases del día específico con detalles
                var clasesProgramadas = await _claseProgramadaRepository
                    .GetClasesPorCursoYDiaAsync(inscripcion.IdCursoAcademico, diaSemana);

                var ahora = DateTime.Now.TimeOfDay;
                var diaActual = DateTime.Now.DayOfWeek;

                // 3. Mapear a DTOs con lógica de clase actual/próxima
                var clasesDto = clasesProgramadas
                    .Select(c =>
                    {
                        var esClaseActual = diaSemana == diaActual &&
                                           ahora >= c.StartTime &&
                                           ahora <= c.EndTime;

                        var esProximaClase = false;
                        if (diaSemana == diaActual && !esClaseActual)
                        {
                            var clasesRestantes = clasesProgramadas
                                .Where(cl => cl.StartTime > ahora)
                                .OrderBy(cl => cl.StartTime)
                                .FirstOrDefault();

                            esProximaClase = clasesRestantes?.Id == c.Id;
                        }

                        return new ClaseHorarioDto
                        {
                            IdClaseProgramada = c.Id,
                            HoraInicio = c.StartTime,
                            HoraFin = c.EndTime,
                            NombreAsignatura = c.AsignaturaNombre,
                            NombreProfesor = c.ProfesorNombreCompleto,
                            Ubicacion = c.Location ?? "N/A",
                            EsClaseActual = esClaseActual,
                            EsProximaClase = esProximaClase
                        };
                    })
                    .ToList();

                return ApiResponse<List<ClaseHorarioDto>>.SuccessResponse(
                    clasesDto,
                    "Clases del día obtenidas exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener clases del día para estudiante {estudianteId}");
                return ApiResponse<List<ClaseHorarioDto>>.ErrorResponse(
                    "Error al obtener las clases del día",
                    new List<string> { ex.Message }
                );
            }
        }

        #region Helpers
        private string ConvertirDayOfWeekAEspanol(DayOfWeek dia)
        {
            return dia switch
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
