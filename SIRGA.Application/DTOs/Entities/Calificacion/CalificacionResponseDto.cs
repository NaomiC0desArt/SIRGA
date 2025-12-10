namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class CalificacionResponseDto
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteMatricula { get; set; }
        public int IdAsignatura { get; set; }
        public string AsignaturaNombre { get; set; }
        public string TipoAsignatura { get; set; }
        public int IdPeriodo { get; set; }
        public int NumeroPeriodo { get; set; }
        public string CursoNombre { get; set; }
        public Dictionary<string, decimal> Componentes { get; set; }
        public decimal Total { get; set; }
        public bool Publicada { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public string Observaciones { get; set; }
    }
}
