using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.IA
{
    public class ActividadRecomendadaDto
    {
        public int IdActividad { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Ubicacion { get; set; }
        public string ColorTarjeta { get; set; }
        public string RutaImagen { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string NombreProfesor { get; set; }

        // Datos de recomendación
        public int CantidadCompañerosInscritos { get; set; }
        public double PorcentajePopularidad { get; set; }
        public string RazonRecomendacion { get; set; }
        public int ScoreRecomendacion { get; set; }
    }
}
