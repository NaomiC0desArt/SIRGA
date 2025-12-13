using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.Interfaces.Entities;
using System.Security.Claims;

namespace SIRGA.API.Controllers.IA
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Estudiante")]
    public class IACalificacionController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;
        private readonly ILogger<IACalificacionController> _logger;

        public IACalificacionController(
            ICalificacionService calificacionService,
            ILogger<IACalificacionController> logger)
        {
            _calificacionService = calificacionService;
            _logger = logger;
        }

        /// <summary>
        /// Genera el mensaje inicial de IA al ver una calificación
        /// </summary>
        [HttpPost("Generar-Mensaje")]
        public async Task<IActionResult> GenerarMensaje([FromBody] GenerarMensajeRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"🤖 Solicitud mensaje IA - Estudiante: {request.EstudianteId}");

                var result = await _calificacionService.GenerarMensajeIAAsync(
                    request.EstudianteId,
                    request.AsignaturaId,
                    request.PeriodoId
                );

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en GenerarMensaje");
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }

        /// <summary>
        /// Responde a un mensaje del estudiante
        /// </summary>
        [HttpPost("Responder")]
        public async Task<IActionResult> Responder([FromBody] ResponderMensajeRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"🤖 Respuesta IA - Estudiante: {request.EstudianteId}");

                var result = await _calificacionService.ResponderMensajeIAAsync(
                    request.EstudianteId,
                    request.AsignaturaId,
                    request.MensajeEstudiante,
                    request.HistorialConversacion ?? new List<string>()
                );

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en Responder");
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }

        [HttpGet("Test-API-Key")]
        [AllowAnonymous] // ✅ Temporal para testing
        public async Task<IActionResult> TestAPIKey()
        {
            try
            {
                _logger.LogInformation("🧪 Probando conexión con Google Gemini...");

                // Obtener la API Key de configuración
                var apiKey = HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["GoogleGemini:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "❌ API Key no configurada en appsettings.json"
                    });
                }

                _logger.LogInformation($"🔑 API Key encontrada: {apiKey.Substring(0, Math.Min(15, apiKey.Length))}...");

                // Crear instancia de GoogleAI
                var googleAI = new Mscc.GenerativeAI.GoogleAI(apiKey);
                var model = googleAI.GenerativeModel(model: "gemini-1.5-flash");

                _logger.LogInformation("📡 Enviando prompt de prueba...");

                // Hacer una prueba simple
                var response = await model.GenerateContent("Di 'Hola' en español");

                if (response != null && !string.IsNullOrWhiteSpace(response.Text))
                {
                    _logger.LogInformation($"✅ Respuesta recibida: {response.Text}");

                    return Ok(new
                    {
                        success = true,
                        message = "✅ Conexión exitosa con Google Gemini",
                        response = response.Text,
                        apiKeyPrefix = apiKey.Substring(0, Math.Min(15, apiKey.Length)) + "..."
                    });
                }

                return Ok(new
                {
                    success = false,
                    message = "⚠️ Respuesta vacía de Gemini"
                });
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "❌ Error HTTP");

                return Ok(new
                {
                    success = false,
                    message = $"❌ Error HTTP: {httpEx.StatusCode}",
                    error = httpEx.Message,
                    details = "Verifica que la API Key sea válida en https://aistudio.google.com/app/apikey"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general");

                return Ok(new
                {
                    success = false,
                    message = "❌ Error general",
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }

        public class GenerarMensajeRequest
        {
            public int EstudianteId { get; set; }
            public int AsignaturaId { get; set; }
            public int PeriodoId { get; set; }
        }

        public class ResponderMensajeRequest
        {
            public int EstudianteId { get; set; }
            public int AsignaturaId { get; set; }
            public string MensajeEstudiante { get; set; }
            public List<string> HistorialConversacion { get; set; }
        }
    }
}
