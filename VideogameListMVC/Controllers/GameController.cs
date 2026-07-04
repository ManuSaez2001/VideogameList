using LogicaAplicacion.InterfacesCasosUso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VideogameListMVC.Controllers {
    [Authorize(Roles ="NormalUser")]
    public class GameController : Controller {
        private readonly ILogger _logger;
        private readonly ICUSearchGames _cuSearchGames;
        public GameController(ILogger<GameController> logger, ICUSearchGames cuSearchGames) {
            _logger = logger;
            _cuSearchGames = cuSearchGames;
        }
        public IActionResult Index() {
            return View();
        }

        public IActionResult SearchGame(string searchTerm) {
            _logger.LogInformation("Searching for game: {SearchTerm}", searchTerm);
            return View();
        }

        /// <summary>
        /// Endpoint AJAX para obtener sugerencias de juegos (para dropdown)
        /// Retorna JSON con id, nombre, rating y URL de portada de los juegos encontrados
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SearchGameSuggestions(string term) {
            _logger.LogInformation("SearchGameSuggestions called with term: {Term}", term);

            if (string.IsNullOrWhiteSpace(term) || term.Length < 2) {
                _logger.LogWarning("Search term too short or empty: {Term}", term);
                return Json(new List<object>());
            }

            try {
                _logger.LogInformation("Calling IIGDBService.GetGameDataWithCovers with term: {Term}", term);

                // Usar el método que obtiene covers expandidos para tener la URL de imagen
                var gamesExpanded = await _cuSearchGames.SearchGamesByNameWithCovers(term);

                _logger.LogInformation("Received {GameCount} games from service", gamesExpanded.Count());

                // Proyectamos id, name, rating y coverUrl para el dropdown (ya vienen ordenados por popularidad)
                var suggestions = gamesExpanded.Select(g => new {
                    id = g.Id,
                    name = g.Name,
                    rating = g.Rating,
                    coverUrl = g.GetCoverUrl()
                }).Take(10).ToList(); // Limitamos a 10 sugerencias

                _logger.LogInformation("Returning {SuggestionCount} suggestions for term: {Term}", suggestions.Count, term);
                return Json(suggestions);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error searching for games with term: {Term}", term);
                return Json(new List<object>());
            }
        }

        /// <summary>
        /// Obtiene los detalles completos de un juego después de seleccionarlo del dropdown
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GameDetail(int id) {
            try {
                var games = await _cuSearchGames.SearchGamesAdvanced(id.ToString());
                var game = games.FirstOrDefault();

                if (game == null) {
                    return NotFound();
                }

                _logger.LogInformation("Retrieved game detail for id: {GameId}", id);
                return View(game);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving game detail for id: {GameId}", id);
                return StatusCode(500);
            }
        }
    }
}
