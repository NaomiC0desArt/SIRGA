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

        // Estudiante
        public string EstudianteMatricula { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteApellido { get; set; }

        public string EstudianteNombreCompleto => $"{EstudianteNombre} {EstudianteApellido}";

        // Curso
        public string GradoNombre { get; set; }
        public string SeccionNombre { get; set; }
        public string AnioEscolarPeriodo { get; set; }

        public string CursoNombre => $"{GradoNombre} - Sección {SeccionNombre} ({AnioEscolarPeriodo})";
    }
}
