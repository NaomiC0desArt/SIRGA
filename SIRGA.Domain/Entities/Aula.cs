
using SIRGA.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class Aula
    {
        public int Id { get; set; }
        public string Codigo { get; set; }        // "A-101", "LAB-QUI-01"
        public string Nombre { get; set; }         // "Aula 101", "Laboratorio de Química"
        public TipoEspacio Tipo { get; set; }
        public int Capacidad { get; set; }
        public bool EstaDisponible { get; set; }

        [NotMapped]
        public string NombreCompleto => $"{Codigo} - {Nombre}";
    }
}
