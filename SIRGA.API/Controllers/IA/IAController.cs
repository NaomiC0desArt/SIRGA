using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.IA;
using SIRGA.Application.Services.IA;
using System.Security.Claims;
using SIRGA.Application.Interfaces.Services;

namespace SIRGA.API.Controllers.IA
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IAController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly IActividadRecomendadorService _recomendadorService;
        public IAController(
            IChatbotService chatbotService,
            IActividadRecomendadorService recomendadorService)
        {
            _chatbotService = chatbotService;
            _recomendadorService = recomendadorService;
        }

        // ==================== CHATBOT ENDPOINTS ====================

        /// <summary>
        /// Procesa una consulta del chatbot
        /// </summary>
        [HttpPost("Chatbot/Consulta")]
        public async Task<IActionResult> ProcesarConsulta([FromBody] ChatbotRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener el ApplicationUserId del usuario autenticado
            request.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _chatbotService.ProcesarConsultaAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Genera un resumen académico del estudiante autenticado
        /// </summary>
        [Authorize(Roles = "Estudiante")]
        [HttpGet("Chatbot/Mi-Resumen")]
        public async Task<IActionResult> GenerarMiResumen([FromQuery] int idEstudiante)
        {
            var result = await _chatbotService.GenerarResumenAcademicoAsync(idEstudiante);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene actividades recomendadas para un estudiante
        /// </summary>
        [Authorize(Roles = "Estudiante")]
        [HttpGet("Recomendador/Actividades/{idEstudiante:int}")]
        public async Task<IActionResult> RecomendarActividades(int idEstudiante)
        {
            var result = await _recomendadorService.RecomendarActividadesAsync(idEstudiante);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene estadísticas de popularidad de actividades
        /// </summary>
        [Authorize(Roles = "Admin,Profesor")]
        [HttpGet("Recomendador/Estadisticas/{idCursoAcademico:int}")]
        public async Task<IActionResult> ObtenerEstadisticas(int idCursoAcademico)
        {
            var result = await _recomendadorService.ObtenerEstadisticasActividadesAsync(idCursoAcademico);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
