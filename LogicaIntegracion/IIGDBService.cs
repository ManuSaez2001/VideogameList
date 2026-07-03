using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaIntegracion {
    public interface IIGDBService {
        Task<IEnumerable<DTOIGDBGame>> GetGameData(string search, string fields);
        Task<IEnumerable<DTOIGDBCover>> GetGameCover(IEnumerable<DTOIGDBGame> game);
        Task<IEnumerable<DTOIGDBGameExpanded>> GetGameDataWithCovers(string search);
    }
}
