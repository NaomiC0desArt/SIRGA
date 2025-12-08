using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.IA;

namespace SIRGA.Application.Interfaces.Services
{
    public interface IChatbotService
    {
        /// Procesa una consulta del estudiante y genera una respuesta contextual
        Task<ApiResponse<ChatbotResponseDto>> ProcesarConsultaAsync(ChatbotRequestDto request);
        /// Genera un resumen académico personalizado del estudiante
        Task<ApiResponse<string>> GenerarResumenAcademicoAsync(int idEstudiante);
    }
}
