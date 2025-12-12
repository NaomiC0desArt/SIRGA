namespace SIRGA.Web.Models.Calificacion
{
    public class HistorialCalificacionDto
    {
        public int Id { get; set; }
        public int IdCalificacion { get; set; }
        public int NumeroPeriodo { get; set; }
        public string ValoresAnteriores { get; set; }
        public string ValoresNuevos { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioRol { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string MotivoEdicion { get; set; }
        public string CambiosRealizados { get; set; }
    }
}
