namespace SIRGA.Web.Models.Calificacion
{
    public class CalificacionEstudianteViewDto
    {
        public string AsignaturaNombre { get; set; }
        public string TipoAsignatura { get; set; }
        public List<CalificacionPorPeriodoDto> Periodos { get; set; }
        public decimal PromedioGeneral { get; set; }
    }
}
