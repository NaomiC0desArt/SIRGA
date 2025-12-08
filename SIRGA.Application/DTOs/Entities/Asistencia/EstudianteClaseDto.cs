

namespace SIRGA.Application.DTOs.Entities.Asistencia
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

        // Info de asistencia si ya fue registrada
        public int? AsistenciaId { get; set; }
        public string? EstadoAsistencia { get; set; }
        public bool YaRegistrada { get; set; } = false;
    }
}
