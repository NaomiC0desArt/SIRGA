namespace SIRGA.Web.Models.Calificacion
{
    public class EstudianteBusquedaDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Matricula { get; set; }
        public string Email { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public int IdCursoAcademico { get; set; }
        public string CursoAcademico { get; set; }
    }
}
