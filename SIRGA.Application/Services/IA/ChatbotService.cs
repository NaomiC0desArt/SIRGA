using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.IA;
using SIRGA.Application.Interfaces.Services;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIRGA.Application.Services.IA
{
    public class ChatbotService : IChatbotService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChatbotService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ChatbotService(
            ApplicationDbContext context,
            IEstudianteRepository estudianteRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<ChatbotService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _estudianteRepository = estudianteRepository;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ApiResponse<ChatbotResponseDto>> ProcesarConsultaAsync(ChatbotRequestDto request)
        {
            try
            {
                // Obtener contexto del estudiante si está disponible
                string contextoEstudiante = "";
                if (request.IdEstudiante.HasValue)
                {
                    contextoEstudiante = await ObtenerContextoEstudianteAsync(request.IdEstudiante.Value);
                }

                // Construir el prompt para Claude
                var systemPrompt = @"Eres un asistente académico del Sistema SIRGA (Sistema de Registro y Gestión Académica). 
Tu objetivo es ayudar a estudiantes con consultas sobre:
- Horarios de clases
- Calificaciones y evaluaciones
- Actividades extracurriculares
- Asistencias
- Información general del sistema

Responde de forma amigable, concisa y profesional. Si no tienes información específica, sugiere al estudiante que consulte con su profesor o administración.";

                var userPrompt = $@"Contexto del estudiante:
{contextoEstudiante}

Pregunta del estudiante:
{request.Pregunta}

Por favor, responde de forma clara y útil.";

                // Llamar a la API de Claude
                var respuestaIA = await LlamarClaudeAPIAsync(systemPrompt, userPrompt);

                var response = new ChatbotResponseDto
                {
                    Respuesta = respuestaIA,
                    Contexto = "Respuesta generada por IA",
                    FechaHora = DateTime.Now,
                    RequiereAccionAdicional = false,
                    AccionSugerida = null
                };

                return ApiResponse<ChatbotResponseDto>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar consulta del chatbot");
                return ApiResponse<ChatbotResponseDto>.ErrorResponse(
                    "Error al procesar la consulta",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> GenerarResumenAcademicoAsync(int idEstudiante)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(idEstudiante);
                if (estudiante == null)
                {
                    return ApiResponse<string>.ErrorResponse("Estudiante no encontrado");
                }

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);

                // Obtener estadísticas
                var totalAsistencias = await _context.Asistencias
                    .Where(a => a.IdEstudiante == idEstudiante)
                    .CountAsync();

                var ausencias = await _context.Asistencias
                    .Where(a => a.IdEstudiante == idEstudiante && a.Estado == "Ausente")
                    .CountAsync();

                var actividadesInscritas = await _context.InscripcionesActividades
                    .Include(ia => ia.Actividad)
                    .Where(ia => ia.IdEstudiante == idEstudiante && ia.EstaActiva)
                    .CountAsync();

                var porcentajeAsistencia = totalAsistencias > 0
                    ? Math.Round((double)(totalAsistencias - ausencias) / totalAsistencias * 100, 1)
                    : 0;

                var prompt = $@"Genera un resumen académico profesional y motivador para el estudiante {user.FirstName} {user.LastName} con las siguientes estadísticas:

- Asistencias registradas: {totalAsistencias}
- Ausencias: {ausencias}
- Porcentaje de asistencia: {porcentajeAsistencia}%
- Actividades extracurriculares activas: {actividadesInscritas}

El resumen debe:
1. Ser profesional pero motivador
2. Destacar logros positivos
3. Sugerir áreas de mejora si es necesario
4. Máximo 150 palabras
5. En español";

                var resumen = await LlamarClaudeAPIAsync(
                    "Eres un consejero académico que genera reportes motivadores para estudiantes.",
                    prompt);

                return ApiResponse<string>.SuccessResponse(resumen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar resumen académico");
                return ApiResponse<string>.ErrorResponse(
                    "Error al generar el resumen",
                    new List<string> { ex.Message });
            }
        }

        // ==================== MÉTODOS PRIVADOS ====================

        private async Task<string> ObtenerContextoEstudianteAsync(int idEstudiante)
        {
            try
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(idEstudiante);
                if (estudiante == null) return "Estudiante no encontrado";

                var user = await _userManager.FindByIdAsync(estudiante.ApplicationUserId);

                var inscripcion = await _context.Inscripciones
                    .Include(i => i.CursoAcademico)
                    .ThenInclude(ca => ca.Grado)
                    .Where(i => i.IdEstudiante == idEstudiante)
                    .OrderByDescending(i => i.FechaInscripcion)
                    .FirstOrDefaultAsync();

                var totalAsistencias = await _context.Asistencias
                    .Where(a => a.IdEstudiante == idEstudiante)
                    .CountAsync();

                var actividadesInscritas = await _context.InscripcionesActividades
                    .Where(ia => ia.IdEstudiante == idEstudiante && ia.EstaActiva)
                    .CountAsync();

                var contexto = new StringBuilder();
                contexto.AppendLine($"Nombre: {user.FirstName} {user.LastName}");
                contexto.AppendLine($"Matrícula: {estudiante.Matricula}");

                if (inscripcion != null)
                {
                    contexto.AppendLine($"Grado: {inscripcion.CursoAcademico.Grado.GradeName} - {inscripcion.CursoAcademico.Grado.Section}");
                    contexto.AppendLine($"Año académico: {inscripcion.CursoAcademico.SchoolYear}");
                }

                contexto.AppendLine($"Asistencias registradas: {totalAsistencias}");
                contexto.AppendLine($"Actividades extracurriculares: {actividadesInscritas}");

                return contexto.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contexto del estudiante");
                return "Error al obtener contexto";
            }
        }

        private async Task<string> LlamarClaudeAPIAsync(string systemPrompt, string userPrompt)
        {
            try
            {
                var apiKey = _configuration["AnthropicAPI:ApiKey"];

                // MODO DESARROLLO: Si no hay API key, retornar respuesta simulada
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("API Key de Anthropic no configurada. Usando respuesta simulada.");
                    return GenerarRespuestaSimulada(userPrompt);
                }

                var requestBody = new
                {
                    model = "claude-sonnet-4-20250514",
                    max_tokens = 500,
                    system = systemPrompt,
                    messages = new[]
                    {
                        new { role = "user", content = userPrompt }
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json")
                };

                request.Headers.Add("x-api-key", apiKey);
                request.Headers.Add("anthropic-version", "2023-06-01");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error API Claude: {response.StatusCode} - {errorContent}");
                    return "Lo siento, hubo un error al procesar tu consulta. Por favor, intenta de nuevo.";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseContent);

                var respuesta = jsonResponse.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                return respuesta ?? "No se pudo generar una respuesta.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error llamando a Claude API");
                return "Lo siento, hubo un error al procesar tu consulta. Por favor, intenta de nuevo más tarde.";
            }
        }

        private string GenerarRespuestaSimulada(string pregunta)
        {
            // Respuestas predefinidas para demostración sin API key
            var preguntaLower = pregunta.ToLower();

            if (preguntaLower.Contains("horario"))
                return "Puedes consultar tu horario de clases en la sección 'Mi Horario' del menú principal. Allí encontrarás todas tus clases organizadas por día y hora.";

            if (preguntaLower.Contains("calificacion") || preguntaLower.Contains("nota"))
                return "Tus calificaciones están disponibles en la sección 'Mis Calificaciones'. Recuerda que se actualizan después de cada evaluación.";

            if (preguntaLower.Contains("actividad") || preguntaLower.Contains("extracurricular"))
                return "Puedes explorar y inscribirte en actividades extracurriculares desde la sección 'Bienestar Estudiantil'. Hay actividades de deportes, arte, y más.";

            if (preguntaLower.Contains("asistencia"))
                return "Tu asistencia se registra automáticamente en cada clase. Si tienes dudas sobre tus ausencias, puedes consultar con tu profesor o la administración.";

            return "Gracias por tu pregunta. Para obtener información específica, te recomiendo visitar las diferentes secciones del sistema o consultar con tu profesor. ¿Hay algo más en lo que pueda ayudarte?";
        }
    }
}
