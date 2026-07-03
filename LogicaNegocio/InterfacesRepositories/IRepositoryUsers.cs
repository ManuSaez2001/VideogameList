using LogicaNegocio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.InterfacesRepositories {
    public interface IRepositoryUsers {
        void Add(User user);
        User Login(string mail);
        User FindById(int id);
        void Remove(int id);
        void Update(User updatedUser);
        List<User> FindAll(int ammount);
        List<User> FindSuggestions(string partialQuery, int ammount);
    }
}
