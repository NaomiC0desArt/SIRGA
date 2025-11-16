namespace SIRGA.Web.Models.Inscripcion
{
    public class InscripcionDto
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteMatricula { get; set; }
        public int IdCursoAcademico { get; set; }
        public string CursoAcademicoNombre { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
