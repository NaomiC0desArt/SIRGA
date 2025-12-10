namespace SIRGA.Web.Models.Calificacion
{
    public class CalificacionEstudianteDto
    {
        public int IdEstudiante { get; set; }
        public string Matricula { get; set; }
        public string NombreCompleto { get; set; }

        // CAMBIAR ESTO:
        // De propiedades individuales a un diccionario:
        public Dictionary<int, decimal?> Componentes { get; set; }

        public string Observaciones { get; set; }
    }
}
