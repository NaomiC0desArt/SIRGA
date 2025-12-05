using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.ActividadExtracurricular
{
    public class EstudianteInscritoDto
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; }
        public string Matricula { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
