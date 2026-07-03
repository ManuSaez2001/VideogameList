using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAplicacion.InterfacesCasosUso {
    public interface ICUSearchGames {

        Task<IEnumerable<DTOIGDBGame>> SearchGamesByName(string searchTerm);

        Task<IEnumerable<DTOIGDBGameExpanded>> SearchGamesByNameWithCovers(string searchTerm);

        Task<IEnumerable<DTOIGDBGame>> GetAllGames(string fields = "id,name,rating,cover,first_release_date,summary,category");

        Task<IEnumerable<DTOIGDBGame>> SearchGamesAdvanced(string searchTerm, string fields = "id,name,rating,cover,first_release_date,summary,category");
    }
}
