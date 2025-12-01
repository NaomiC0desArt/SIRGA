namespace SIRGA.Application.DTOs.ResponseDto
{
    public class CalificacionResponseDto 
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int AsignaturaId { get; set; }
        public string AsignaturaNombre { get; set; }
        public int CursoAcademicoId { get; set; }
        public int PeriodoId { get; set; }
        public string PeriodoNumero { get; set; }

        // TEÓRICA
        public decimal? Tareas { get; set; }
        public decimal? ExamenesTeoricos { get; set; }
        public decimal? Exposiciones { get; set; }
        public decimal? Participacion { get; set; }

        // PRÁCTICA
        public decimal? Practicas { get; set; }
        public decimal? ProyectoFinal { get; set; }
        public decimal? Teoria { get; set; }

        // TEÓRICO-PRÁCTICA
        public decimal? Examenes { get; set; }
        public decimal? Proyectos { get; set; }

        // TOTAL PERIODO
        public decimal? NotaPeriodo { get; set; }

        public bool Publicado { get; set; }
    }
}
