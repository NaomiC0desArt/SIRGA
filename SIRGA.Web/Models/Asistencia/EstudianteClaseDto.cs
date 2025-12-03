namespace SIRGA.Web.Models.Asistencia
{
    public class EstudianteClaseDto
    {
        public int IdEstudiante { get; set; }
        public string Matricula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto => $"{Apellido}, {Nombre}";
        public string? Email { get; set; }
        public string? Photo { get; set; }

        public int? AsistenciaId { get; set; }
        public string? EstadoAsistencia { get; set; }
        public bool YaRegistrada { get; set; } = false;
    }
}
