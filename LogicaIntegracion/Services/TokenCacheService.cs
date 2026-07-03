using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LogicaIntegracion.Services {
    /// <summary>
    /// Servicio para cachear tokens de Twitch y reutilizarlos mientras sean válidos
    /// Evita hacer requests innecesarios de autenticación
    /// </summary>
    public class TokenCacheService {
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly IGDBSettings _settings;
        private readonly ILogger<TokenCacheService> _logger;

        private const string TOKEN_CACHE_KEY = "igdb_access_token";

        public TokenCacheService(
            IMemoryCache memoryCache,
            HttpClient httpClient,
            IGDBSettings settings,
            ILogger<TokenCacheService> logger) {
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un token válido, usando cache si existe y no ha expirado
        /// </summary>
        public async Task<string> GetValidTokenAsync() {
            // 1. Intentar obtener del cache
            if (_memoryCache.TryGetValue(TOKEN_CACHE_KEY, out var cachedToken)) {
                _logger.LogInformation("Token obtenido del cache");
                return (string)cachedToken;
            }

            _logger.LogInformation("Token no en cache, obteniendo nuevo de Twitch");

            // 2. Obtener nuevo token de Twitch
            var newToken = await GetAccessTokenFromTwitch();

            if (string.IsNullOrEmpty(newToken)) {
                _logger.LogError("Falló obtener token de Twitch");
                return null;
            }

            // 3. Cachear el token por un tiempo (los tokens de IGDB son válidos ~5-6 días)
            // Cacheamos por 1 hora como medida de seguridad
            var cacheOptions = new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };

            _memoryCache.Set(TOKEN_CACHE_KEY, newToken, cacheOptions);
            _logger.LogInformation("Token cacheado por 1 hora");

            return newToken;
        }

        /// <summary>
        /// Obtiene un token fresco de Twitch (sin usar cache)
        /// </summary>
        private async Task<string> GetAccessTokenFromTwitch() {
            try {
                string requestUrl = $"https://id.twitch.tv/oauth2/token?client_id={_settings.ClientId}&client_secret={_settings.ClientSecret}&grant_type=client_credentials";

                var tokenResponse = await _httpClient.PostAsync(requestUrl, null);
                var tokenContent = tokenResponse.Content;
                string body = await tokenContent.ReadAsStringAsync();

                _logger.LogInformation("Respuesta de Twitch: {Body}", body);

                if (!tokenResponse.IsSuccessStatusCode) {
                    _logger.LogError("Falló obtener token de Twitch: {StatusCode} - {Body}", tokenResponse.StatusCode, body);
                    return null;
                }

                var jsonObject = JsonConvert.DeserializeObject<dynamic>(body);
                string accessToken = jsonObject?.access_token;

                if (string.IsNullOrEmpty(accessToken)) {
                    _logger.LogError("Token no encontrado en respuesta de Twitch");
                    return null;
                }

                _logger.LogInformation("Token obtenido exitosamente de Twitch");
                return accessToken;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error obteniendo token de Twitch");
                return null;
            }
        }

        /// <summary>
        /// Invalida el token en cache (para reintentos en caso de error 401)
        /// </summary>
        public void InvalidateToken() {
            _memoryCache.Remove(TOKEN_CACHE_KEY);
            _logger.LogInformation("Token invalidado del cache");
        }
    }
}
