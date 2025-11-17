using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Repositories
{
    public class ClaseProgramadaConDetalles
    {
        public int Id { get; set; }
        public int IdAsignatura { get; set; }
        public int IdProfesor { get; set; }
        public int IdCursoAcademico { get; set; }

        // Datos
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public string Location { get; set; }

        // Detalles (desde JOINS)
        public string AsignaturaNombre { get; set; }
        public string ProfesorFirstName { get; set; }
        public string ProfesorLastName { get; set; }
        public string GradoNombre { get; set; }
        public string GradoSeccion { get; set; }
        public string SchoolYear { get; set; }
    }
}
