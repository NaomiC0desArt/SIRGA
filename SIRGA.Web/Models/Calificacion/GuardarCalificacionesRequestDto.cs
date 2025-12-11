namespace SIRGA.Web.Models.Calificacion
{
    public class GuardarCalificacionesRequestDto
    {
        public int IdAsignatura { get; set; }
        public int IdCursoAcademico { get; set; }
        public int IdPeriodo { get; set; }
        public int IdProfesor { get; set; }
        public string TipoAsignatura { get; set; }
        public List<ComponenteDto> Componentes { get; set; }
        public List<GuardarCalificacionDto> Calificaciones { get; set; }
    }
}
