namespace SIRGA.Web.Models.Calificacion
{
    public class EditarCalificacionViewModel
    {
        public int IdEstudiante { get; set; }
        public int IdAsignatura { get; set; }
        public int IdPeriodo { get; set; }
        public string AsignaturaNombre { get; set; }
        public Dictionary<string, decimal?> Componentes { get; set; }
        public decimal TotalActual { get; set; }
    }
}
