using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace LogicaIntegracion.Services {
    /// <summary>
    /// Servicio para implementar retry logic con backoff exponencial
    /// Usa Polly para manejar fallos temporales (429, 5xx)
    /// </summary>
    public class RetryPolicyService {
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly ILogger<RetryPolicyService> _logger;
        private readonly int _maxRetries;
        private readonly int _backoffMs;

        public RetryPolicyService(
            IOptions<IGDBRateLimitingOptions> options,
            ILogger<RetryPolicyService> logger) {
            _logger = logger;
            _maxRetries = options.Value.RetryMaxAttempts;
            _backoffMs = options.Value.RetryBackoffMs;

            // Crear política de retry con backoff exponencial
            _retryPolicy = BuildRetryPolicy();
        }

        /// <summary>
        /// Construye la política de retry usando Polly
        /// Reintenta en: 429 (Too Many Requests), 500, 502, 503, 504
        /// Usa backoff exponencial: 1s, 2s, 4s, etc.
        /// </summary>
        private IAsyncPolicy<HttpResponseMessage> BuildRetryPolicy() {
            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => 
                    r.StatusCode == HttpStatusCode.TooManyRequests || // 429
                    r.StatusCode == HttpStatusCode.InternalServerError || // 500
                    r.StatusCode == HttpStatusCode.BadGateway || // 502
                    r.StatusCode == HttpStatusCode.ServiceUnavailable || // 503
                    r.StatusCode == HttpStatusCode.GatewayTimeout) // 504
                .WaitAndRetryAsync<HttpResponseMessage>(
                    retryCount: _maxRetries,
                    sleepDurationProvider: retryAttempt => {
                        // Backoff exponencial: 1s, 2s, 4s, 8s...
                        var delay = TimeSpan.FromMilliseconds(_backoffMs * Math.Pow(2, retryAttempt - 1));
                        _logger.LogWarning(
                            "Reintento {Attempt} de {MaxRetries}, esperando {DelayMs}ms",
                            retryAttempt,
                            _maxRetries,
                            delay.TotalMilliseconds);
                        return delay;
                    },
                    onRetryAsync: (outcome, timespan, retryCount, context) => {
                        if (outcome.Exception != null) {
                            _logger.LogWarning(
                                outcome.Exception,
                                "Error en intento {RetryCount}: {Exception}",
                                retryCount,
                                outcome.Exception.Message);
                        }
                        else {
                            _logger.LogWarning(
                                "Response con status {StatusCode} en intento {RetryCount}",
                                outcome.Result?.StatusCode,
                                retryCount);
                        }
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        /// Ejecuta una acción HTTP con retry policy
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteHttpAsync(Func<Task<HttpResponseMessage>> action) {
            return await _retryPolicy.ExecuteAsync(action);
        }
    }
}
