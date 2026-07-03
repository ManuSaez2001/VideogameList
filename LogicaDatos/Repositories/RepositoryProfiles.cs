using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Repositories {
    public class RepositoryProfiles : IRepositoryProfile {
        public VGListContext Context { get; set; }
        public RepositoryProfiles(VGListContext context) { Context = context; }
        public void Add(Profile profile) {
            Context.Profiles.Add(profile);
            Context.SaveChanges();
        }

        public void Remove(int id) {
            Profile toErase = FindById(id);
            if (toErase == null) throw new Exception("The provided Id does not correspond to any Profile");
            Context.Profiles.Remove(toErase);
            Context.SaveChanges();
        }

        public void Update(Profile profile) {
            Context.Profiles.Update(profile);
            Context.SaveChanges();
        }

        public Profile FindById(int id) {
            return Context.Profiles
                .Where(p => p.Id == id)
                .SingleOrDefault();
        }
    }
}
