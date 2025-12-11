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
        private readonly JsonSerializerOptions _jsonOptions;

        // Endpoints que NO requieren autenticación
        private static readonly HashSet<string> PublicEndpoints = new(StringComparer.OrdinalIgnoreCase)
        {
            "api/Auth/Login",
            "api/Auth/register",
            "api/Auth/Forgot-Password",
            "api/Auth/Reset-Password",
            "api/Auth/Confirm-Email",
            "api/Auth/User-Exists"
        };

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Crea un HttpClient con o sin autenticación según el endpoint
        /// </summary>
        private HttpClient CreateClient(string endpoint)
        {
            var client = _httpClientFactory.CreateClient("SIRGA_API");

            // Si es un endpoint público, NO agregar token
            if (IsPublicEndpoint(endpoint))
            {
                _logger.LogDebug($"📖 Endpoint público: {endpoint} (sin token)");
                return client;
            }

            // Para endpoints protegidos, intentar agregar token
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");

            if (!string.IsNullOrEmpty(token))
            {
                if (IsTokenExpired(token))
                {
                    _logger.LogWarning("⚠️ Token expirado detectado - será removido");
                    _httpContextAccessor.HttpContext.Session.Remove("JWTToken");
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug($"🔐 Token adjunto al request");
                }
            }
            else
            {
                _logger.LogDebug("ℹ️ No hay token en sesión");
            }

            return client;
        }

        /// <summary>
        /// Verifica si un endpoint es público (no requiere autenticación)
        /// </summary>
        private bool IsPublicEndpoint(string endpoint)
        {
            return PublicEndpoints.Any(publicEndpoint =>
                endpoint.StartsWith(publicEndpoint, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Maneja respuestas 401/403 de forma centralizada
        /// </summary>
        private void HandleUnauthorizedResponse(string endpoint, System.Net.HttpStatusCode statusCode)
        {
            _logger.LogWarning($"🚫 {statusCode} en {endpoint}");

            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                throw new UnauthorizedAccessException("El token ha expirado o no es válido.");
            }
        }

        // ==================== MÉTODOS PÚBLICOS ====================

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var client = CreateClient(endpoint);
                _logger.LogDebug($"➡️ GET {endpoint}");

                var response = await client.GetAsync(endpoint);
                _logger.LogDebug($"⬅️ Response: {(int)response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    HandleUnauthorizedResponse(endpoint, response.StatusCode);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"⚠️ GET {endpoint} failed: {response.StatusCode}. Content: {content}");
                    return default;
                }

                var body = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(body, _jsonOptions);
            }
            catch (UnauthorizedAccessException)
            {
                throw; // Re-lanzar para que el controlador lo maneje
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en GET {endpoint}");
                throw;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient(endpoint);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogDebug($"➡️ POST {endpoint}");
                var response = await client.PostAsync(endpoint, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"⬅️ Response: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    HandleUnauthorizedResponse(endpoint, response.StatusCode);
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"⚠️ POST {endpoint} failed: {response.StatusCode} - {responseContent}");
                    return default;
                }

                return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en POST {endpoint}");
                throw;
            }
        }

        public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient(endpoint);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogDebug($"➡️ PUT {endpoint}");
                var response = await client.PutAsync(endpoint, content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HandleUnauthorizedResponse(endpoint, response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en PUT {endpoint}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var client = CreateClient(endpoint);
                _logger.LogDebug($"➡️ DELETE {endpoint}");
                var response = await client.DeleteAsync(endpoint);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HandleUnauthorizedResponse(endpoint, response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en DELETE {endpoint}");
                return false;
            }
        }

        public async Task<bool> PatchAsync(string endpoint)
        {
            try
            {
                var client = CreateClient(endpoint);
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint);

                _logger.LogDebug($"➡️ PATCH {endpoint}");
                var response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HandleUnauthorizedResponse(endpoint, response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en PATCH {endpoint}");
                return false;
            }
        }

        public async Task<bool> PatchAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient(endpoint);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                _logger.LogDebug($"➡️ PATCH {endpoint}");
                var response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HandleUnauthorizedResponse(endpoint, response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en PATCH {endpoint}");
                return false;
            }
        }

        // ==================== MÉTODOS PRIVADOS ====================

        /// <summary>
        /// Verifica si un token JWT ha expirado
        /// </summary>
        private bool IsTokenExpired(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    _logger.LogWarning("⚠️ Token JWT con formato inválido");
                    return true;
                }

                var payloadBase64 = parts[1];
                var base64 = payloadBase64.Replace('-', '+').Replace('_', '/');

                // Agregar padding si es necesario
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(base64);
                var jsonPayload = Encoding.UTF8.GetString(jsonBytes);

                using var document = JsonDocument.Parse(jsonPayload);
                if (document.RootElement.TryGetProperty("exp", out var expElement))
                {
                    var expirationTimeUnix = expElement.GetInt64();
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeUnix);

                    // Margen de 5 minutos antes de expiración
                    var isExpired = expirationTime.Subtract(TimeSpan.FromMinutes(5)) < DateTimeOffset.UtcNow;

                    if (isExpired)
                    {
                        _logger.LogWarning($"⏰ Token expirado. Expira: {expirationTime:yyyy-MM-dd HH:mm:ss UTC}");
                    }

                    return isExpired;
                }

                _logger.LogWarning("⚠️ Token sin claim 'exp'");
                return true; // Si no tiene 'exp', asumir expirado
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al parsear token JWT");
                return true; // Asumir expirado si hay error
            }
        }

    }
}

