namespace SIRGA.Web.Models.Calificacion
{
    public class EditarCalificacionDto
    {
        public int IdCalificacion { get; set; }
        public string MotivoModificacion { get; set; }
        public CalificacionEstudianteDto NuevaCalificacion { get; set; }
    }
}
