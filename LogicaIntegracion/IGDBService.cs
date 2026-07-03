using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using LogicaIntegracion.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LogicaIntegracion {
    public class IGDBService : IIGDBService {
        private readonly HttpClient _httpClient;
        private readonly IGDBSettings _settings;
        private readonly ILogger<IGDBService> _logger;
        private readonly TokenCacheService _tokenCache;
        private readonly RateLimitService _rateLimiter;
        private readonly RetryPolicyService _retryPolicy;
        private readonly SearchCacheService _searchCache;

        public IGDBService(
            HttpClient httpClient,
            IOptions<IGDBSettings> settings,
            ILogger<IGDBService> logger,
            TokenCacheService tokenCache,
            RateLimitService rateLimiter,
            RetryPolicyService retryPolicy,
            SearchCacheService searchCache) {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _tokenCache = tokenCache;
            _rateLimiter = rateLimiter;
            _retryPolicy = retryPolicy;
            _searchCache = searchCache;
        }

        public async Task<IEnumerable<DTOIGDBGame>> GetGameData(string search, string fields) {
            try {
                // 1. Aplicar rate limiting antes de hacer el request
                await _rateLimiter.WaitAsync();

                // 2. Obtener token usando cache
                var accessToken = await _tokenCache.GetValidTokenAsync();
                if (string.IsNullOrEmpty(accessToken)) {
                    _logger.LogError("Failed to obtain access token from Twitch");
                    return Enumerable.Empty<DTOIGDBGame>();
                }

                // 3. Limpiar headers anteriores para evitar duplicados
                _httpClient.DefaultRequestHeaders.Remove("Client-ID");
                _httpClient.DefaultRequestHeaders.Remove("Authorization");

                _httpClient.DefaultRequestHeaders.Add("Client-ID", _settings.ClientId);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // 4. Query IGDB v4: Remover el filtro 'where category = 0' que puede ser problemático
                var requestBody = $"search \"{search}\"; fields {fields}; limit 10;";
                _logger.LogInformation("IGDB Request Body: {RequestBody}", requestBody);

                // 5. Ejecutar request con retry policy
                var response = await _retryPolicy.ExecuteHttpAsync(async () => {
                    var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");
                    return await _httpClient.PostAsync(_settings.Url + "games", content);
                });

                string dataString = HerramientasAPI.LeerContenidoRespuesta(response);
                _logger.LogInformation("IGDB Response: {Response}", dataString);

                if (!response.IsSuccessStatusCode) {
                    // Si error 401, invalidar token para próximo intento
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                        _tokenCache.InvalidateToken();
                    }
                    _logger.LogError("IGDB API Error: {StatusCode} - {Response}", response.StatusCode, dataString);
                    return Enumerable.Empty<DTOIGDBGame>();
                }

                if (string.IsNullOrWhiteSpace(dataString) || dataString == "[]") {
                    _logger.LogInformation("IGDB returned empty result for search: {Search}", search);
                    return Enumerable.Empty<DTOIGDBGame>();
                }

                var games = JsonConvert.DeserializeObject<IEnumerable<DTOIGDBGame>>(dataString);
                _logger.LogInformation("Successfully parsed {Count} games from IGDB", games?.Count() ?? 0);
                return games ?? Enumerable.Empty<DTOIGDBGame>();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving game data from IGDB for search: {Search}", search);
                return Enumerable.Empty<DTOIGDBGame>();
            }
        }

        /// <summary>
        /// Obtiene datos de juegos con información de cover expandida (incluye image_id)
        /// Útil para construir URLs de imágenes directamente
        /// Utiliza cache de búsquedas para evitar llamadas repetidas
        /// </summary>
        public async Task<IEnumerable<DTOIGDBGameExpanded>> GetGameDataWithCovers(string search) {
            try {
                // 1. Intentar obtener del cache de búsquedas
                if (_searchCache.TryGetCachedResults(search, out var cachedResults)) {
                    return cachedResults;
                }

                // 2. Aplicar rate limiting
                await _rateLimiter.WaitAsync();

                // 3. Obtener token usando cache
                var accessToken = await _tokenCache.GetValidTokenAsync();
                if (string.IsNullOrEmpty(accessToken)) {
                    _logger.LogError("Failed to obtain access token from Twitch");
                    return Enumerable.Empty<DTOIGDBGameExpanded>();
                }

                // 4. Limpiar headers anteriores
                _httpClient.DefaultRequestHeaders.Remove("Client-ID");
                _httpClient.DefaultRequestHeaders.Remove("Authorization");

                _httpClient.DefaultRequestHeaders.Add("Client-ID", _settings.ClientId);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // 5. Query con cover expandido: obtiene cover.image_id directamente
                var fields = "id,name,rating,cover.image_id";
                var requestBody = $"search \"{search}\"; fields {fields}; limit 10;";
                _logger.LogInformation("IGDB Request Body (with covers): {RequestBody}", requestBody);

                // 6. Ejecutar request con retry policy
                var response = await _retryPolicy.ExecuteHttpAsync(async () => {
                    var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");
                    return await _httpClient.PostAsync(_settings.Url + "games", content);
                });

                string dataString = HerramientasAPI.LeerContenidoRespuesta(response);
                _logger.LogInformation("IGDB Response (with covers): {Response}", dataString);

                if (!response.IsSuccessStatusCode) {
                    // Si error 401, invalidar token para próximo intento
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                        _tokenCache.InvalidateToken();
                    }
                    _logger.LogError("IGDB API Error: {StatusCode} - {Response}", response.StatusCode, dataString);
                    return Enumerable.Empty<DTOIGDBGameExpanded>();
                }

                if (string.IsNullOrWhiteSpace(dataString) || dataString == "[]") {
                    _logger.LogInformation("IGDB returned empty result for search: {Search}", search);
                    return Enumerable.Empty<DTOIGDBGameExpanded>();
                }

                var games = JsonConvert.DeserializeObject<IEnumerable<DTOIGDBGameExpanded>>(dataString);
                var gamesList = games ?? Enumerable.Empty<DTOIGDBGameExpanded>();

                // 7. Cachear los resultados para futuras búsquedas
                _searchCache.CacheResults(search, gamesList);

                _logger.LogInformation("Successfully parsed {Count} games with covers from IGDB", gamesList.Count());
                return gamesList;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving game data with covers from IGDB for search: {Search}", search);
                return Enumerable.Empty<DTOIGDBGameExpanded>();
            }
        }

        public async Task<IEnumerable<DTOIGDBCover>> GetGameCover(IEnumerable<DTOIGDBGame> game) {
            if (game.Count() == 0) {
                return Enumerable.Empty<DTOIGDBCover>();
            }

            try {
                // 1. Aplicar rate limiting
                await _rateLimiter.WaitAsync();

                // 2. Obtener token usando cache
                var accessToken = await _tokenCache.GetValidTokenAsync();
                if (string.IsNullOrEmpty(accessToken)) {
                    _logger.LogError("Failed to obtain access token for cover request");
                    return Enumerable.Empty<DTOIGDBCover>();
                }

                // 3. Limpiar headers anteriores
                _httpClient.DefaultRequestHeaders.Remove("Client-ID");
                _httpClient.DefaultRequestHeaders.Remove("Authorization");

                _httpClient.DefaultRequestHeaders.Add("Client-ID", _settings.ClientId);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // 4. Query para obtener cover
                var requestBody = $"fields image_id; where id={game.FirstOrDefault().Cover};";

                // 5. Ejecutar request con retry policy
                var response = await _retryPolicy.ExecuteHttpAsync(async () => {
                    var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");
                    return await _httpClient.PostAsync(_settings.Url + "covers", content);
                });

                string dataString = HerramientasAPI.LeerContenidoRespuesta(response);

                if (!response.IsSuccessStatusCode) {
                    // Si error 401, invalidar token
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                        _tokenCache.InvalidateToken();
                    }
                    _logger.LogError("IGDB Cover API Error: {StatusCode} - {Response}", response.StatusCode, dataString);
                    return Enumerable.Empty<DTOIGDBCover>();
                }

                var cover = JsonConvert.DeserializeObject<IEnumerable<DTOIGDBCover>>(dataString);
                return cover ?? Enumerable.Empty<DTOIGDBCover>();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving game cover from IGDB");
                return Enumerable.Empty<DTOIGDBCover>();
            }
        }

            }
        }
