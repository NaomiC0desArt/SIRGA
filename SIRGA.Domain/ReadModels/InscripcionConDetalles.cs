using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.ReadModels
{
    public class InscripcionConDetalles
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public int IdCursoAcademico { get; set; }
        public DateTime FechaInscripcion { get; set; }

        // datos del estudiante
        public string EstudianteMatricula { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteApellido { get; set; }

        // datos del curso
        public string GradoNombre { get; set; }
        public string GradoSeccion { get; set; }
        public string SchoolYear { get; set; }

        public string EstudianteNombreCompleto => $"{EstudianteNombre} {EstudianteApellido}";
        public string CursoNombre => $"{GradoNombre} {GradoSeccion} - {SchoolYear}";
    }
}
