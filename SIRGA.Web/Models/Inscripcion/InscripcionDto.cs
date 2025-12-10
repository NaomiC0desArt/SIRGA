namespace SIRGA.Web.Models.Inscripcion
{
    public class InscripcionDto
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public int IdCursoAcademico { get; set; }
        public DateTime FechaInscripcion { get; set; }

        // Detalles del estudiante
        public string EstudianteNombre { get; set; }
        public string EstudianteMatricula { get; set; }

        // Detalles del curso
        public string CursoAcademicoNombre { get; set; }

        // Para navegación si es necesario
        public object Estudiante { get; set; }
        public object CursoAcademico { get; set; }
    }
}
