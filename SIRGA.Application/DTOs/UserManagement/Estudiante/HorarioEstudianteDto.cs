using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.UserManagement.Estudiante
{
    public class HorarioEstudianteDto
    {
        public string DiaSemana { get; set; }
        public List<ClaseHorarioDto> Clases { get; set; } = new();
    }

    public class ClaseHorarioDto
    {
        public int IdClaseProgramada { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string NombreAsignatura { get; set; }
        public string NombreProfesor { get; set; }
        public string Ubicacion { get; set; }

        // Para indicar si es la clase actual o próxima
        public bool EsClaseActual { get; set; }
        public bool EsProximaClase { get; set; }
    }

    public class HorarioSemanalDto
    {
        public List<HorarioEstudianteDto> HorarioSemanal { get; set; } = new();
        public string PeriodoAcademico { get; set; }
        public string GradoSeccion { get; set; }
    }
}
