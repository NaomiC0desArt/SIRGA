namespace SIRGA.Application.Interfaces.IA
{
    public interface IIACalificacionService
    {
        Task<string> GenerarMensajeInicialAsync(
            string nombreEstudiante,
            string asignatura,
            string tipoAsignatura,
            Dictionary<string, decimal> componentes,
            decimal totalCalificacion);

        Task<string> ResponderEstudianteAsync(
            string nombreEstudiante,
            string asignatura,
            string mensajeEstudiante,
            List<string> historialConversacion);
    }
}
