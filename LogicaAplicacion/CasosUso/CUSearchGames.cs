using DTOs;
using LogicaAplicacion.InterfacesCasosUso;
using LogicaIntegracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAplicacion.CasosUso {
    public class CUSearchGames : ICUSearchGames {
        private readonly IIGDBService _gameService;

        public CUSearchGames(IIGDBService gameService) {
            _gameService = gameService;
        }
        public async Task<IEnumerable<DTOIGDBGame>> SearchGamesByName(string searchTerm) {
            if (string.IsNullOrWhiteSpace(searchTerm)) {
                return Enumerable.Empty<DTOIGDBGame>();
            }

            // Campos básicos que funcionan correctamente en IGDB API v4
            var fields = "id,name,rating,cover";
            var results = await _gameService.GetGameData(searchTerm.Trim(), fields);

            // Ordenar por popularidad (rating descendente) y luego por nombre
            return results
                .OrderByDescending(g => g.Rating ?? 0)
                .ThenBy(g => g.Name)
                .ToList();
        }

        /// <summary>
        /// Búsqueda por nombre con datos de cover expandidos para obtener URLs de imagen
        /// </summary>
        public async Task<IEnumerable<DTOIGDBGameExpanded>> SearchGamesByNameWithCovers(string searchTerm) {
            if (string.IsNullOrWhiteSpace(searchTerm)) {
                return Enumerable.Empty<DTOIGDBGameExpanded>();
            }

            var results = await _gameService.GetGameDataWithCovers(searchTerm.Trim());

            // Ordenar por popularidad (rating descendente) y luego por nombre
            return results
                .OrderByDescending(g => g.Rating ?? 0)
                .ThenBy(g => g.Name)
                .ToList();
        }

        public async Task<IEnumerable<DTOIGDBGame>> GetAllGames(string fields = "id,name,rating,cover") {
            // Búsqueda general sin término específico usando wildcard
            var searchTerm = "*";
            var results = await _gameService.GetGameData(searchTerm, fields);

            // Ordenar por popularidad (rating descendente)
            return results
                .OrderByDescending(g => g.Rating ?? 0)
                .ThenBy(g => g.Name)
                .ToList();
        }

        public async Task<IEnumerable<DTOIGDBGame>> SearchGamesAdvanced(string searchTerm, string fields = "id,name,rating,cover") {
            if (string.IsNullOrWhiteSpace(searchTerm)) {
                return Enumerable.Empty<DTOIGDBGame>();
            }

            var results = await _gameService.GetGameData(searchTerm.Trim(), fields);

            // Ordenar por popularidad (rating descendente) y luego por nombre
            return results
                .OrderByDescending(g => g.Rating ?? 0)
                .ThenBy(g => g.Name)
                .ToList();
        }
    }
}
