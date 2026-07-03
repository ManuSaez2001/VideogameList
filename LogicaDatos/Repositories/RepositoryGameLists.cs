using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Repositories {
    public class RepositoryGameLists : IRepositoryGameLists {
        public VGListContext Context { get; set; }
        public RepositoryGameLists(VGListContext context) { Context = context; }
        public void Add(GameList gameList) {
            Context.GameLists.Add(gameList);
            Context.SaveChanges();
        }
    }
}
