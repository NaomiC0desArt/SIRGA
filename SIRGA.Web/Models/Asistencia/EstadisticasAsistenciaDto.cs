namespace SIRGA.Web.Models.Asistencia
{
    public class EstadisticasAsistenciaDto
    {
        public int TotalClases { get; set; }
        public int TotalPresentes { get; set; }
        public int TotalAusentes { get; set; }
        public int TotalTardes { get; set; }
        public int TotalJustificados { get; set; }
        public decimal PorcentajeAsistencia { get; set; }

        public string PorcentajeFormateado => $"{PorcentajeAsistencia:F1}%";
    }
}
