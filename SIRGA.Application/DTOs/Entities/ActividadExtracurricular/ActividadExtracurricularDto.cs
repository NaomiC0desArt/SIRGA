using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.ActividadExtracurricular
{
    public class ActividadExtracurricularDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Requisitos { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string DiaSemana { get; set; }
        public string Ubicacion { get; set; }
        public string ColorTarjeta { get; set; }
        public string RutaImagen { get; set; }
        public bool EstaActiva { get; set; }
        public int IdProfesorEncargado { get; set; }
        public string NombreProfesor { get; set; }
        public int TotalInscritos { get; set; }
        public bool EstaInscrito { get; set; }
    }
}
