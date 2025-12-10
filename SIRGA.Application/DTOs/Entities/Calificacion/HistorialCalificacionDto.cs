namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class HistorialCalificacionDto
    {
        public int Id { get; set; }
        public string UsuarioModificador { get; set; }
        public string RolModificador { get; set; }
        public string MotivoModificacion { get; set; }
        public decimal TotalAnterior { get; set; }
        public decimal TotalNuevo { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Dictionary<string, decimal> ComponentesAnteriores { get; set; }
        public Dictionary<string, decimal> ComponentesNuevos { get; set; }
    }
}
