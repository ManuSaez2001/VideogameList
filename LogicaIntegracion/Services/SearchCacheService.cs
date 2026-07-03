using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicaIntegracion.Services {
    /// <summary>
    /// Servicio para cachear resultados de búsquedas de IGDB
    /// Evita llamadas repetidas al API para la misma búsqueda
    /// Cache se mantiene por 30 minutos (configurable)
    /// </summary>
    public class SearchCacheService {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<SearchCacheService> _logger;
        private readonly int _cacheTtlMinutes;

        private const string CACHE_KEY_PREFIX = "igdb_search_";

        public SearchCacheService(
            IMemoryCache memoryCache,
            IOptions<IGDBRateLimitingOptions> options,
            ILogger<SearchCacheService> logger) {
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheTtlMinutes = options.Value.SearchCacheTtlMinutes;
        }

        /// <summary>
        /// Intenta obtener resultados del cache
        /// </summary>
        public bool TryGetCachedResults(
            string searchTerm,
            out IEnumerable<DTOIGDBGameExpanded> results) {
            var cacheKey = NormalizeCacheKey(searchTerm);

            var found = _memoryCache.TryGetValue(cacheKey, out var cachedData);

            if (found) {
                results = (List<DTOIGDBGameExpanded>)cachedData;
                _logger.LogInformation("Cache HIT para búsqueda: {SearchTerm}", searchTerm);
            }
            else {
                results = null;
                _logger.LogInformation("Cache MISS para búsqueda: {SearchTerm}", searchTerm);
            }

            return found;
        }

        /// <summary>
        /// Cachea resultados de búsqueda
        /// </summary>
        public void CacheResults(
            string searchTerm,
            IEnumerable<DTOIGDBGameExpanded> results) {
            var cacheKey = NormalizeCacheKey(searchTerm);

            var cacheOptions = new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheTtlMinutes)
            };

            _memoryCache.Set(cacheKey, results.ToList(), cacheOptions);
            _logger.LogInformation(
                "Resultados cacheados para: {SearchTerm} ({Count} juegos, TTL: {TtlMinutes}min)",
                searchTerm,
                results.Count(),
                _cacheTtlMinutes);
        }

        /// <summary>
        /// Limpia el cache completo
        /// </summary>
        public void ClearAll() {
            // Nota: IMemoryCache no tiene método Clear() built-in
            // En producción, podrías usar un diccionario paralelo para rastrear claves
            _logger.LogWarning("ClearAll solicitado, pero IMemoryCache no soporta Clear global");
        }

        /// <summary>
        /// Limpia resultados de una búsqueda específica
        /// </summary>
        public void RemoveSearchCache(string searchTerm) {
            var cacheKey = NormalizeCacheKey(searchTerm);
            _memoryCache.Remove(cacheKey);
            _logger.LogInformation("Cache limpiado para: {SearchTerm}", searchTerm);
        }

        /// <summary>
        /// Normaliza la clave de cache (lowercase, sin espacios extras)
        /// </summary>
        private string NormalizeCacheKey(string searchTerm) {
            var normalized = searchTerm.ToLower().Trim();
            return $"{CACHE_KEY_PREFIX}{normalized}";
        }
    }
}
