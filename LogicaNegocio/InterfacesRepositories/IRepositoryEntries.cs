using LogicaNegocio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.InterfacesRepositories {
    public interface IRepositoryEntries {
        public void Add(Entry entry);
        public void Update(Entry entry);
        public Entry FindById(int id);
        public List<Entry> FindAll(int ammount);
        public List<Entry> FindByUser(User owner);
        public void Remove(int id);
    }
}
