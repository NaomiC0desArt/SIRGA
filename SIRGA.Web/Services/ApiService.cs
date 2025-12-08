using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace SIRGA.Web.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiService> _logger;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("SIRGA_API");

            // Obtener el token de la sesión
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            _logger.LogInformation($"Token presente: {!string.IsNullOrEmpty(token)}");

            if (!string.IsNullOrEmpty(token))
            {
                if (IsTokenExpired(token))
                {
                    // El token está caducado, se fuerza el cierre de sesión y se lanza una excepción.
                    _logger.LogWarning("JWTToken expirado. Forzando logout.");

                    // Eliminar el token de la sesión para forzar el re-login.
                    _httpContextAccessor.HttpContext.Session.Remove("JWTToken");

                    // Aquí puedes lanzar una excepción o redirigir al login si estás en un contexto de controlador
                    token = null;
                }

                if (!string.IsNullOrEmpty(token)) {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    _logger.LogInformation($"JWT Token in session: {token}");
                }
                    
            }
            else
            {
                _logger.LogWarning("No se encontró JWTToken en la sesión al crear el cliente.");
            }

            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var client = CreateClient();
                var fullUrl = $"{client.BaseAddress}{endpoint}";
                _logger.LogInformation($"➡️ Calling GET {fullUrl}");

                var response = await client.GetAsync(endpoint);

                _logger.LogInformation($"⬅️ Response from {fullUrl}: {(int)response.StatusCode} {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"❌ GET {endpoint} failed with {response.StatusCode}. Content: {content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        // Limpia la sesión y lanza la excepción para que el controlador la atrape.
                        _httpContextAccessor.HttpContext.Session.Remove("JWTToken");
                        throw new UnauthorizedAccessException("El token ha expirado o no es válido. Requiere re-login.");
                    }

                    return default;
                }

                var body = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception during GET {endpoint}");
                return default;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"POST to {endpoint}");
                var response = await client.PostAsync(endpoint, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response: {response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"POST {endpoint} failed: {responseContent}");
                    return default;
                }

                return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in POST {endpoint}");
                throw;
            }
        }

        public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(endpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in PUT {endpoint}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var client = CreateClient();
                var response = await client.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DELETE {endpoint}");
                return false;
            }
        }

        public async Task<bool> PatchAsync(string endpoint)
        {
            try
            {
                var client = CreateClient();
                _logger.LogInformation($"➡️ Calling PATCH {endpoint}");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint);
                var response = await client.SendAsync(request);

                _logger.LogInformation($"⬅️ PATCH Response: {(int)response.StatusCode} {response.ReasonPhrase}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in PATCH {endpoint}");
                return false;
            }
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                // Extrae el payload del token (la segunda parte entre puntos)
                var payloadBase64 = token.Split('.')[1];

                // El payload debe tener un padding adecuado para ser decodificado.
                // Se añade relleno si es necesario.
                var base64 = payloadBase64.Replace('-', '+').Replace('_', '/');
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                // Decodifica el payload
                var jsonBytes = Convert.FromBase64String(base64);
                var jsonPayload = Encoding.UTF8.GetString(jsonBytes);

                // Deserializa el payload para obtener el claim 'exp'
                using (var document = JsonDocument.Parse(jsonPayload))
                {
                    if (document.RootElement.TryGetProperty("exp", out var expElement))
                    {
                        var expirationTimeUnix = expElement.GetInt64();

                        // Convierte el tiempo Unix a DateTimeOffset
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeUnix);

                        // Devuelve true si la hora de caducidad es anterior a la hora actual.
                        // Se resta 1 minuto de seguridad (opcional).
                        return expirationTime.Subtract(TimeSpan.FromMinutes(1)) < DateTimeOffset.UtcNow;
                    }
                }
                return true; // Asumir expirado si no se puede leer
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al parsear el token JWT para verificar caducidad.");
                return true; // Asumir expirado en caso de error
            }
        }

        public async Task<bool> PatchAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"➡️ Calling PATCH {endpoint}");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                var response = await client.SendAsync(request);

                _logger.LogInformation($"⬅️ PATCH Response: {(int)response.StatusCode} {response.ReasonPhrase}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in PATCH {endpoint}");
                return false;
            }
        }

        
    }
}

