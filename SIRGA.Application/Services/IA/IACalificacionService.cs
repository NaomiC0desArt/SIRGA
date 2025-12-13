
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mscc.GenerativeAI;
using SIRGA.Application.Interfaces.IA;
using System.Text;

namespace SIRGA.Application.Services.IA
{
    public class IACalificacionService : IIACalificacionService
    {
        private readonly string _apiKey;
        private readonly ILogger<IACalificacionService> _logger;
        private readonly GoogleAI _googleAI;

        public IACalificacionService(
            IConfiguration configuration,
            ILogger<IACalificacionService> logger)
        {
            _apiKey = configuration["GoogleGemini:ApiKey"]
                ?? throw new ArgumentException("Google Gemini API Key no configurada");

            _logger = logger;
            _googleAI = new GoogleAI(_apiKey);

            _logger.LogInformation("✅ IACalificacionService inicializado correctamente");
            _logger.LogInformation($"🔑 API Key configurada: {_apiKey.Substring(0, Math.Min(10, _apiKey.Length))}...");
        }

        public async Task<string> GenerarMensajeInicialAsync(
            string nombreEstudiante,
            string asignatura,
            string tipoAsignatura,
            Dictionary<string, decimal> componentes,
            decimal totalCalificacion)
        {
            try
            {
                var componentesBajos = AnalizarComponentes(componentes, tipoAsignatura);
                var nivelRendimiento = ClasificarRendimiento(totalCalificacion);

                var prompt = ConstruirPromptInicial(
                    nombreEstudiante,
                    asignatura,
                    tipoAsignatura,
                    componentesBajos,
                    nivelRendimiento,
                    totalCalificacion
                );

                _logger.LogInformation($"🤖 Generando mensaje IA para {nombreEstudiante} en {asignatura}");
                _logger.LogDebug($"📝 Prompt completo: {prompt}");

                // ✅ CORRECCIÓN: Usar el modelo correcto con configuración explícita
                var model = _googleAI.GenerativeModel(model: "gemini-2.5-flash");

                _logger.LogInformation("📡 Enviando request a Google Gemini...");

                var response = await model.GenerateContent(prompt);

                _logger.LogInformation($"✅ Respuesta recibida de Gemini");

                if (response == null || string.IsNullOrWhiteSpace(response.Text))
                {
                    _logger.LogWarning("⚠️ Respuesta vacía de Gemini");
                    return $"Hola {nombreEstudiante}, veo que obtuviste {totalCalificacion} puntos en {asignatura}. ¿Hay algo en lo que pueda ayudarte?";
                }

                _logger.LogInformation($"✅ Mensaje IA generado exitosamente. Longitud: {response.Text.Length} caracteres");
                _logger.LogDebug($"📄 Respuesta: {response.Text}");

                return response.Text;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "❌ Error HTTP al conectar con Google Gemini");
                _logger.LogError($"StatusCode: {httpEx.StatusCode}");
                _logger.LogError($"Mensaje: {httpEx.Message}");

                if (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError("🔴 ERROR 404: Verifica que la API Key sea válida y que el modelo 'gemini-1.5-flash' esté disponible");
                }

                return $"Hola {nombreEstudiante}, veo que obtuviste {totalCalificacion} puntos en {asignatura}. ¿Cómo te sientes con este resultado?";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general al generar mensaje inicial de IA");
                _logger.LogError($"Tipo de error: {ex.GetType().Name}");
                _logger.LogError($"Mensaje: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                return $"Hola {nombreEstudiante}, veo que obtuviste {totalCalificacion} puntos en {asignatura}. ¿Hay algo en lo que pueda ayudarte?";
            }
        }

        public async Task<string> ResponderEstudianteAsync(
            string nombreEstudiante,
            string asignatura,
            string mensajeEstudiante,
            List<string> historialConversacion)
        {
            try
            {
                var prompt = ConstruirPromptRespuesta(
                    nombreEstudiante,
                    asignatura,
                    mensajeEstudiante,
                    historialConversacion
                );

                _logger.LogInformation($"🤖 Respondiendo a {nombreEstudiante}");
                _logger.LogDebug($"📝 Mensaje estudiante: {mensajeEstudiante}");
                _logger.LogDebug($"📝 Prompt completo: {prompt}");

                // ✅ CORRECCIÓN: Usar el modelo correcto con configuración explícita
                var model = _googleAI.GenerativeModel(model: "gemini-2.5-flash");

                _logger.LogInformation("📡 Enviando request a Google Gemini...");

                var response = await model.GenerateContent(prompt);

                _logger.LogInformation($"✅ Respuesta recibida de Gemini");

                if (response == null || string.IsNullOrWhiteSpace(response.Text))
                {
                    _logger.LogWarning("⚠️ Respuesta vacía de Gemini");
                    return "Lo siento, no pude procesar tu mensaje. ¿Podrías reformularlo?";
                }

                _logger.LogInformation($"✅ Respuesta IA generada exitosamente");
                _logger.LogDebug($"📄 Respuesta: {response.Text}");

                return response.Text;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "❌ Error HTTP al conectar con Google Gemini");
                _logger.LogError($"StatusCode: {httpEx.StatusCode}");
                _logger.LogError($"Mensaje: {httpEx.Message}");

                if (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError("🔴 ERROR 404: Verifica que la API Key sea válida y que el modelo 'gemini-1.5-flash' esté disponible");
                    _logger.LogError($"🔑 API Key usada: {_apiKey.Substring(0, Math.Min(15, _apiKey.Length))}...");
                }
                else if (httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogError("🔴 ERROR 403: La API Key no tiene permisos o está bloqueada");
                }
                else if (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("🔴 ERROR 401: La API Key no es válida");
                }

                return "Disculpa, tuve problemas de conexión con el asistente. Intenta de nuevo en un momento.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general al responder mensaje de estudiante");
                _logger.LogError($"Tipo de error: {ex.GetType().Name}");
                _logger.LogError($"Mensaje: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                return "Disculpa, tuve un problema procesando tu mensaje. ¿Podrías reformularlo?";
            }
        }

        // ==================== MÉTODOS PRIVADOS ====================

        private List<string> AnalizarComponentes(
            Dictionary<string, decimal> componentes,
            string tipoAsignatura)
        {
            var componentesBajos = new List<string>();

            var umbrales = new Dictionary<string, decimal>
            {
                { "Tareas", 30m },
                { "Exámenes Teóricos", 18m },
                { "Exámenes", 21m },
                { "Exposiciones", 14m },
                { "Prácticas", 30m },
                { "Proyecto Final", 20m },
                { "Proyectos", 14m },
                { "Participación", 7m },
                { "Teoría", 7m }
            };

            foreach (var comp in componentes)
            {
                if (umbrales.TryGetValue(comp.Key, out var umbral))
                {
                    if (comp.Value < umbral)
                    {
                        componentesBajos.Add(comp.Key);
                    }
                }
            }

            return componentesBajos;
        }

        private string ClasificarRendimiento(decimal total)
        {
            if (total >= 90) return "excelente";
            if (total >= 80) return "muy bueno";
            if (total >= 70) return "aprobado";
            if (total >= 60) return "bajo";
            return "crítico";
        }

        private string ConstruirPromptInicial(
            string nombre,
            string asignatura,
            string tipoAsignatura,
            List<string> componentesBajos,
            string nivelRendimiento,
            decimal totalCalificacion)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente pedagógico empático y motivador llamado 'Asistente Académico SIRGA'.");
            sb.AppendLine("Tu objetivo es ayudar a estudiantes a mejorar su rendimiento académico.");
            sb.AppendLine();
            sb.AppendLine("INSTRUCCIONES:");
            sb.AppendLine("1. Saluda al estudiante por su nombre de manera cálida");
            sb.AppendLine("2. Menciona la asignatura y su calificación de forma constructiva");
            sb.AppendLine("3. Si hay componentes bajos, pregunta específicamente sobre ellos");
            sb.AppendLine("4. Muestra empatía y ofrece ayuda sin juzgar");
            sb.AppendLine("5. Tu mensaje debe ser BREVE (máximo 4-5 líneas)");
            sb.AppendLine("6. Usa un tono amigable y cercano, como un mentor");
            sb.AppendLine();
            sb.AppendLine("CONTEXTO DEL ESTUDIANTE:");
            sb.AppendLine($"- Nombre: {nombre}");
            sb.AppendLine($"- Asignatura: {asignatura} ({tipoAsignatura})");
            sb.AppendLine($"- Calificación total: {totalCalificacion} puntos");
            sb.AppendLine($"- Nivel de rendimiento: {nivelRendimiento}");

            if (componentesBajos.Any())
            {
                sb.AppendLine($"- Componentes con bajo rendimiento: {string.Join(", ", componentesBajos)}");
            }

            sb.AppendLine();
            sb.AppendLine("Genera un mensaje inicial personalizado:");

            return sb.ToString();
        }

        private string ConstruirPromptRespuesta(
            string nombre,
            string asignatura,
            string mensajeEstudiante,
            List<string> historial)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente pedagógico empático. Continúa la conversación con el estudiante.");
            sb.AppendLine();
            sb.AppendLine("INSTRUCCIONES:");
            sb.AppendLine("1. Responde de manera natural y empática");
            sb.AppendLine("2. Da consejos prácticos y específicos");
            sb.AppendLine("3. Si el estudiante parece satisfecho o la conversación ha avanzado suficiente (3-4 intercambios), despídete motivándolo");
            sb.AppendLine("4. Mantén respuestas BREVES (máximo 4-5 líneas)");
            sb.AppendLine("5. Ofrece técnicas de estudio concretas cuando sea apropiado");
            sb.AppendLine();
            sb.AppendLine($"Estudiante: {nombre}");
            sb.AppendLine($"Asignatura: {asignatura}");
            sb.AppendLine();

            if (historial.Any())
            {
                sb.AppendLine("HISTORIAL DE LA CONVERSACIÓN:");
                foreach (var mensaje in historial.Take(10)) // Limitar a últimos 10 mensajes
                {
                    sb.AppendLine(mensaje);
                }
                sb.AppendLine();
            }

            sb.AppendLine($"ÚLTIMO MENSAJE DEL ESTUDIANTE: {mensajeEstudiante}");
            sb.AppendLine();
            sb.AppendLine("Responde de manera apropiada:");

            return sb.ToString();
        }
    }
}