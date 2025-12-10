using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.ReadModels
{
    public class ClaseConDetallesParaHorario
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public string Location { get; set; }

        public int IdAsignatura { get; set; }
        public string AsignaturaNombre { get; set; }

        public int IdProfesor { get; set; }
        public string ProfesorNombre { get; set; }
        public string ProfesorApellido { get; set; }

        public int IdCursoAcademico { get; set; }
        public string GradoNombre { get; set; }
        public string SeccionNombre { get; set; }
        public string AnioEscolarPeriodo { get; set; }

        public string ProfesorNombreCompleto => $"{ProfesorNombre} {ProfesorApellido}";
        
    }
}
