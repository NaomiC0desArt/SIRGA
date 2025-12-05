namespace SIRGA.Web.Models.ActividadExtracurricular
{
    public class ActividadViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Requisitos { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string DiaSemana { get; set; }
        public string Ubicacion { get; set; }
        public string ColorTarjeta { get; set; }
        public string RutaImagen { get; set; }
        public bool EstaActiva { get; set; }
        public int IdProfesorEncargado { get; set; }
        public string NombreProfesor { get; set; }
        public int TotalInscritos { get; set; }
        public bool EstaInscrito { get; set; }

        public string HoraInicioFormateada
        {
            get
            {
                // Convertir TimeSpan a DateTime para formatear con AM/PM
                var hora = DateTime.Today.Add(HoraInicio);
                return hora.ToString("h:mm tt"); // Ejemplo: 2:30 PM
            }
        }

        public string HoraFinFormateada
        {
            get
            {
                // Convertir TimeSpan a DateTime para formatear con AM/PM
                var hora = DateTime.Today.Add(HoraFin);
                return hora.ToString("h:mm tt"); // Ejemplo: 4:30 PM
            }
        }
    }
}
