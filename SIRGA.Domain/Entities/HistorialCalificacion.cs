using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class HistorialCalificacion
    {
        public int Id { get; set; }

        // Relación con Calificación
        public int IdCalificacion { get; set; }

        // Período que se modificó
        public int NumeroPeriodo { get; set; }

        // Valores anteriores (JSON serializado)
        public string ValoresAnteriores { get; set; }

        // Valores nuevos (JSON serializado)
        public string ValoresNuevos { get; set; }

        // Información del usuario que hizo el cambio
        public string UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioRol { get; set; }

        // Motivo del cambio
        public string MotivoEdicion { get; set; }

        // Fecha del cambio
        public DateTime FechaModificacion { get; set; }

        // Navigation Properties
        [ForeignKey("IdCalificacion")]
        public Calificacion Calificacion { get; set; }

        [NotMapped]
        public string CambiosRealizados
        {
            get
            {
                if (string.IsNullOrEmpty(ValoresAnteriores) || string.IsNullOrEmpty(ValoresNuevos))
                    return "Sin cambios registrados";

                return $"Período {NumeroPeriodo}: Modificado por {UsuarioNombre}";
            }
        }
    }
}
