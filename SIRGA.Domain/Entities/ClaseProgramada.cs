using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class ClaseProgramada
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek  WeekDay { get; set; }
        public string Location { get; set; } // Aula, Laboratorio, etc

        public int IdAsignatura { get; set; }
        [ForeignKey("IdAsignatura")]
        public Asignatura Asignatura { get; set; }

        public int IdProfesor { get; set; }
        [ForeignKey("IdProfesor")]
        public Profesor Profesor { get; set; }

        public int IdCursoAcademico { get; set; }
        [ForeignKey("IdCursoAcademico")]
        public CursoAcademico CursoAcademico { get; set; }
    }
}
