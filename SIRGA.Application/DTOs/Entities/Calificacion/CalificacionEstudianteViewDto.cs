namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class CalificacionEstudianteViewDto
    {
        public int IdCalificacion { get; set; }
        public int IdAsignatura { get; set; } // AGREGAR ESTA PROPIEDAD
        public string AsignaturaNombre { get; set; }
        public string TipoAsignatura { get; set; }
        public List<PeriodoCalificacionViewDto> Periodos { get; set; }
        public decimal PromedioGeneral { get; set; }
    }
}
