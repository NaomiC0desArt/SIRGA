using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.ReadModels
{
    public class InscripcionActividadConDetalles
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public int IdActividad { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public bool EstaActiva { get; set; }

        // Estudiante
        public string EstudianteMatricula { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteApellido { get; set; }

        // Actividad
        public string ActividadNombre { get; set; }
        public string ActividadCategoria { get; set; }

        public string EstudianteNombreCompleto => $"{EstudianteNombre} {EstudianteApellido}";
    }
}
