namespace SIRGA.Web.Models.Inscripcion
{
    public class CreateInscripcionDto
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public int IdCursoAcademico { get; set; }
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}
