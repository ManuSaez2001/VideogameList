using LogicaNegocio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.InterfacesRepositories {
    public interface IRepositoryUsers {
        Task AddAsync(User user);
        Task<User> GetByMailAsync(string mail);
        Task<User> FindByIdAsync(int id);
        Task RemoveAsync(int id);
        Task UpdateAsync(User updatedUser);
        Task<List<User>> FindAllAsync(int ammount);
        Task<List<User>> FindSuggestionsAsync(string partialQuery, int ammount);
    }
}
