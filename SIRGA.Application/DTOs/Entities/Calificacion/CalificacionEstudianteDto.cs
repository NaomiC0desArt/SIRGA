namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class CalificacionEstudianteDto
    {
        public int IdEstudiante { get; set; }
        public string Matricula { get; set; }
        public string NombreCompleto { get; set; }

        // Cambiamos a un diccionario: ComponenteId -> Valor
        public Dictionary<int, decimal?> Componentes { get; set; }

        public string Observaciones { get; set; }
    }
}
