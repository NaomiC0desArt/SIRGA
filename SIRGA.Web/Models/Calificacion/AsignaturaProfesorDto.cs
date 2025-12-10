namespace SIRGA.Web.Models.Calificacion
{
    public class AsignaturaProfesorDto
    {
        public int IdAsignatura { get; set; }
        public string AsignaturaNombre { get; set; }
        public string TipoAsignatura { get; set; }
        public int IdCursoAcademico { get; set; }
        public string CursoNombre { get; set; }
        public string GradoNombre { get; set; }
        public string SeccionNombre { get; set; }
        public int CantidadEstudiantes { get; set; }
        public int CalificacionesPublicadas { get; set; }
        public int CalificacionesPendientes { get; set; }
    }
}
