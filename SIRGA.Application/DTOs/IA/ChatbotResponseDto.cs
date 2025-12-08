

namespace SIRGA.Application.DTOs.IA
{
    public class ChatbotResponseDto
    {
        public string Respuesta { get; set; }
        public string Contexto { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public bool RequiereAccionAdicional { get; set; }
        public string AccionSugerida { get; set; }
    }
}
