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

            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");

            if (!string.IsNullOrEmpty(token))
            {
                // ✅ Solo verificar expiración si el token existe
                if (IsTokenExpired(token))
                {
                    _logger.LogWarning("⚠️ Token expirado detectado");
                    _httpContextAccessor.HttpContext.Session.Remove("JWTToken");
                    // NO lanzar excepción aquí - dejar que la API responda 401
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug($"✅ Token adjunto al request");
                }
            }
            else
            {
                _logger.LogDebug("ℹ️ No hay token en sesión");
            }

            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var client = CreateClient();
                var fullUrl = $"{client.BaseAddress}{endpoint}";
                _logger.LogDebug($"➡️ GET {fullUrl}");

                var response = await client.GetAsync(endpoint);

                _logger.LogDebug($"⬅️ Response: {(int)response.StatusCode}");

                // ✅ Manejar 401/403 sin lanzar excepción - dejar que el controlador decida
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning($"🚫 {response.StatusCode} en {endpoint}");

                    // Limpiar token solo si es 401
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                        throw new UnauthorizedAccessException("El token ha expirado o no es válido. Requiere re-login.");
                    }

                    return default;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"⚠️ GET {endpoint} failed: {response.StatusCode}. Content: {content}");
                    return default;
                }

                var body = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (UnauthorizedAccessException)
            {
                // Re-lanzar para que el controlador lo maneje
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Exception during GET {endpoint}");
                throw; // Re-lanzar para que el controlador lo maneje
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient();
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogDebug($"➡️ POST {endpoint}");
                var response = await client.PostAsync(endpoint, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"⬅️ Response: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning($"🚫 {response.StatusCode} en POST {endpoint}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                        throw new UnauthorizedAccessException("El token ha expirado o no es válido.");
                    }

                    return default;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"⚠️ POST {endpoint} failed: {responseContent}");
                    return default;
                }

                return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error in POST {endpoint}");
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

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                    throw new UnauthorizedAccessException("El token ha expirado.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error in PUT {endpoint}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var client = CreateClient();
                var response = await client.DeleteAsync(endpoint);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                    throw new UnauthorizedAccessException("El token ha expirado.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error in DELETE {endpoint}");
                return false;
            }
        }

        public async Task<bool> PatchAsync(string endpoint)
        {
            try
            {
                var client = CreateClient();
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint);
                var response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                    throw new UnauthorizedAccessException("El token ha expirado.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error in PATCH {endpoint}");
                return false;
            }
        }

        public async Task<bool> PatchAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var client = CreateClient();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                var response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");
                    throw new UnauthorizedAccessException("El token ha expirado.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error in PATCH {endpoint}");
                return false;
            }
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var payloadBase64 = token.Split('.')[1];
                var base64 = payloadBase64.Replace('-', '+').Replace('_', '/');

                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(base64);
                var jsonPayload = Encoding.UTF8.GetString(jsonBytes);

                using (var document = JsonDocument.Parse(jsonPayload))
                {
                    if (document.RootElement.TryGetProperty("exp", out var expElement))
                    {
                        var expirationTimeUnix = expElement.GetInt64();
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeUnix);

                        // ✅ Margen de 5 minutos antes de expiración
                        var isExpired = expirationTime.Subtract(TimeSpan.FromMinutes(5)) < DateTimeOffset.UtcNow;

                        if (isExpired)
                        {
                            _logger.LogWarning($"⏰ Token expira en: {expirationTime:yyyy-MM-dd HH:mm:ss UTC}");
                        }

                        return isExpired;
                    }
                }

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

