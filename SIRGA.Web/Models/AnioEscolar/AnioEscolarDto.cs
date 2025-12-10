namespace SIRGA.Web.Models.AnioEscolar
{
    public class AnioEscolarDto
    {
        public int Id { get; set; }
        public int AnioInicio { get; set; }
        public int AnioFin { get; set; }
        public bool Activo { get; set; }
        public string? Periodo { get; set; }
    }
}
