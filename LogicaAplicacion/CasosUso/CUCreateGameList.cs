using DTOs;
using LogicaAplicacion.InterfacesCasosUso;
using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAplicacion.CasosUso {
    public class CUCreateGameList : ICUCreateGameList{
        IRepositoryGameLists repoGameLists;
        IRepositoryUsers repoUsers;
        public CUCreateGameList(IRepositoryGameLists repoGameLists, IRepositoryUsers repoUsers) {
            this.repoGameLists = repoGameLists;
            this.repoUsers = repoUsers;
        }

        public async Task CreateGameListAsync(DTOGameList gameListDTO) {
            if (string.IsNullOrWhiteSpace(gameListDTO.Name)) {
                throw new ArgumentException("Name cannot be null or empty", nameof(gameListDTO.Name));
            }
            if (string.IsNullOrWhiteSpace(gameListDTO.Description)) {
                throw new ArgumentException("Description cannot be null or empty", nameof(gameListDTO.Description));
            }
            var owner = await repoUsers.GetByMailAsync(gameListDTO.OwnerEmail);
            if (owner == null) {
                throw new ArgumentException("Owner cannot be null", nameof(gameListDTO.OwnerEmail));
            }
            var gameList = new GameList {
                Name = gameListDTO.Name,
                Description = gameListDTO.Description,
                Owner = owner
            };
            await repoGameLists.AddAsync(gameList);
        }
    }
}
