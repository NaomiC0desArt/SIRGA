using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.ReadModels
{
    public class ActividadConDetalles
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Requisitos { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public string Ubicacion { get; set; }
        public string ColorTarjeta { get; set; }
        public string RutaImagen { get; set; }
        public bool EstaActiva { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Profesor
        public int IdProfesorEncargado { get; set; }
        public string ProfesorNombre { get; set; }
        public string ProfesorApellido { get; set; }

        // Estadísticas
        public int TotalInscritos { get; set; }

        public string ProfesorNombreCompleto => $"{ProfesorNombre} {ProfesorApellido}";
    }
}
