namespace SIRGA.Web.Models.Calificacion
{
    public class GuardarCalificacionDto
    {
        public int IdEstudiante { get; set; }
        public Dictionary<int, decimal?> Componentes { get; set; }
        public string Observaciones { get; set; }
    }
}
