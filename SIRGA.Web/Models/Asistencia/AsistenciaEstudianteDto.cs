namespace SIRGA.Web.Models.Asistencia
{
    public class AsistenciaEstudianteDto
    {
        public int IdEstudiante { get; set; }
        public string Estado { get; set; }
        public string? Observaciones { get; set; }
        public bool RequiereJustificacion { get; set; } = false;
    }
}
