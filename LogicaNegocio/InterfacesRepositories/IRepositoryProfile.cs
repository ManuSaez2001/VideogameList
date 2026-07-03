using LogicaNegocio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.InterfacesRepositories {
    public interface IRepositoryProfile {
        void Add(Profile profile);
        void Remove(int id);
        void Update(Profile profile);
        Profile FindById(int id);
    }
}
