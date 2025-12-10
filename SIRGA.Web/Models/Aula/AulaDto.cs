namespace SIRGA.Web.Models.Aula
{
    public class AulaDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int Capacidad { get; set; }
        public bool EstaDisponible { get; set; }
        public string NombreCompleto { get; set; }
    }
}
