using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Repositories {
    public class RepositoryEntries : IRepositoryEntries {
        public VGListContext Context { get; set; }
        public RepositoryEntries(VGListContext context) { Context = context; }

        public void Add(Entry entry) {
            bool entryExists = Context.Entries.Any(e=> e.Equals(entry));
            if (entryExists) throw new Exception("You already have that Game in your List");
            Context.Entries.Add(entry);
            Context.SaveChanges();
        }

        public void Update(Entry entry) {
            Context.Entries.Update(entry);
            Context.SaveChanges();
        }

        public Entry FindById(int id) {
            return Context.Entries
                .Where(e => e.Id == id)
                .SingleOrDefault();
        }

        public List<Entry> FindAll(int ammount) {
            List<Entry> entries = Context.Entries.Take(ammount).ToList();
            return entries;
        }

        public List<Entry> FindByUser(User user) {
            List<Entry> entries = Context.Entries.Where(e => e.Owner.Equals(user)).ToList();
            return entries;
        }

        public void Remove(int id) {
            Entry toErase = FindById(id);
            if (toErase == null) throw new Exception("The provided Id does not correspond with any Entry");
            Context.Entries.Remove(toErase);
            Context.SaveChanges();
        }
    }
}
