using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicaIntegracion.Services {
    /// <summary>
    /// Servicio para implementar throttling/rate limiting
    /// Limita requests a máximo 4 por segundo (límite de IGDB)
    /// Usa SemaphoreSlim para control concurrente
    /// </summary>
    public class RateLimitService {
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxRequestsPerSecond;
        private readonly ILogger<RateLimitService> _logger;
        private DateTime _windowStartTime = DateTime.UtcNow;

        public RateLimitService(
            IOptions<IGDBRateLimitingOptions> options,
            ILogger<RateLimitService> logger) {
            _maxRequestsPerSecond = options.Value.MaxRequestsPerSecond;
            _logger = logger;
            // Inicializar semaphore con el máximo permitido
            _semaphore = new SemaphoreSlim(_maxRequestsPerSecond, _maxRequestsPerSecond);
        }

        /// <summary>
        /// Espera si es necesario para no exceder el rate limit
        /// Implementa token bucket algorithm
        /// </summary>
        public async Task WaitAsync(CancellationToken cancellationToken = default) {
            // Verificar si la ventana de 1 segundo ha pasado
            var now = DateTime.UtcNow;
            var timeSinceWindowStart = now - _windowStartTime;

            // Si pasó más de 1 segundo, reiniciar la ventana
            if (timeSinceWindowStart.TotalSeconds >= 1) {
                _windowStartTime = now;
                // Restaurar todos los permisos
                for (int i = 0; i < _maxRequestsPerSecond; i++) {
                    _semaphore.Release();
                }
            }

            // Esperar a que haya un permiso disponible
            var acquired = await _semaphore.WaitAsync(100, cancellationToken);

            if (!acquired) {
                _logger.LogWarning("Rate limit alcanzado, esperando...");
                await _semaphore.WaitAsync(cancellationToken);
            }

            _logger.LogDebug("Request permitido por rate limiter");
        }
    }

    /// <summary>
    /// Opciones para configuración de rate limiting
    /// </summary>
    public class IGDBRateLimitingOptions {
        public int MaxRequestsPerSecond { get; set; } = 4;
        public int RetryMaxAttempts { get; set; } = 3;
        public int RetryBackoffMs { get; set; } = 1000;
        public int SearchCacheTtlMinutes { get; set; } = 30;
    }
}
