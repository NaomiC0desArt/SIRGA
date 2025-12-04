using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.UserManagement.Estudiante;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Services
{
    public class HorarioEstudianteService :  IHorarioEstudianteService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IClaseProgramadaRepositoryExtended _claseProgramadaRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HorarioEstudianteService> _logger;

        public HorarioEstudianteService(
            IInscripcionRepository inscripcionRepository,
            IClaseProgramadaRepositoryExtended claseProgramadaRepository,
            ApplicationDbContext context,
            ILogger<HorarioEstudianteService> logger)
        {
            _inscripcionRepository = inscripcionRepository;
            _claseProgramadaRepository = claseProgramadaRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<HorarioSemanalDto>> GetHorarioByEstudianteIdAsync(int estudianteId)
        {
            try
            {
                // 1. Obtener la inscripción activa del estudiante
                var inscripcion = await _inscripcionRepository.GetByConditionAsync(
                    i => i.IdEstudiante == estudianteId
                );

                if (inscripcion == null)
                {
                    return ApiResponse<HorarioSemanalDto>.ErrorResponse(
                        "El estudiante no tiene una inscripción activa"
                    );
                }

                // 2. Obtener todas las clases del curso académico
                var clasesProgramadas = await _claseProgramadaRepository.GetAllByConditionAsync(
                    c => c.IdCursoAcademico == inscripcion.IdCursoAcademico
                );

                if (!clasesProgramadas.Any())
                {
                    return ApiResponse<HorarioSemanalDto>.SuccessResponse(
                        new HorarioSemanalDto
                        {
                            HorarioSemanal = new List<HorarioEstudianteDto>(),
                            PeriodoAcademico = inscripcion.CursoAcademico?.SchoolYear ?? "N/A",
                            GradoSeccion = inscripcion.CursoAcademico?.Grado != null
                                ? $"{inscripcion.CursoAcademico.Grado.GradeName} {inscripcion.CursoAcademico.Grado.Section}"
                                : "N/A"
                        },
                        "No hay clases programadas para este curso"
                    );
                }

                // 3. Obtener información adicional de profesores
                var profesoresIds = clasesProgramadas.Select(c => c.IdProfesor).Distinct().ToList();
                var profesores = await _context.Profesores
                    .Where(p => profesoresIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var profesoresUsers = await _context.Users
                    .Where(u => profesores.Values.Select(p => p.ApplicationUserId).Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id);

                // 4. Organizar por día de la semana
                var diasSemana = new[] {
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
                            var profesor = profesores.GetValueOrDefault(c.IdProfesor);
                            var profesorUser = profesor != null
                                ? profesoresUsers.GetValueOrDefault(profesor.ApplicationUserId)
                                : null;

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
                                NombreAsignatura = c.Asignatura?.Nombre ?? "N/A",
                                NombreProfesor = profesorUser != null
                                    ? $"{profesorUser.FirstName} {profesorUser.LastName}"
                                    : "N/A",
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
                    PeriodoAcademico = inscripcion.CursoAcademico?.SchoolYear ?? "N/A",
                    GradoSeccion = inscripcion.CursoAcademico?.Grado != null
                        ? $"{inscripcion.CursoAcademico.Grado.GradeName} {inscripcion.CursoAcademico.Grado.Section}"
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
                var inscripcion = await _inscripcionRepository.GetByConditionAsync(
                    i => i.IdEstudiante == estudianteId
                );

                if (inscripcion == null)
                {
                    return ApiResponse<List<ClaseHorarioDto>>.ErrorResponse(
                        "El estudiante no tiene una inscripción activa"
                    );
                }

                var clasesProgramadas = await _claseProgramadaRepository.GetAllByConditionAsync(
                    c => c.IdCursoAcademico == inscripcion.IdCursoAcademico &&
                         c.WeekDay == diaSemana
                );

                var profesoresIds = clasesProgramadas.Select(c => c.IdProfesor).Distinct().ToList();
                var profesores = await _context.Profesores
                    .Where(p => profesoresIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var profesoresUsers = await _context.Users
                    .Where(u => profesores.Values.Select(p => p.ApplicationUserId).Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id);

                var ahora = DateTime.Now.TimeOfDay;
                var diaActual = DateTime.Now.DayOfWeek;

                var clasesDto = clasesProgramadas
                    .OrderBy(c => c.StartTime)
                    .Select(c =>
                    {
                        var profesor = profesores.GetValueOrDefault(c.IdProfesor);
                        var profesorUser = profesor != null
                            ? profesoresUsers.GetValueOrDefault(profesor.ApplicationUserId)
                            : null;

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
                            NombreAsignatura = c.Asignatura?.Nombre ?? "N/A",
                            NombreProfesor = profesorUser != null
                                ? $"{profesorUser.FirstName} {profesorUser.LastName}"
                                : "N/A",
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
    }
}
