using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositories;
using LogicaNegocio.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Repositories {
    public class RepositoryUsers : IRepositoryUsers {
        public VGListContext Context { get; set; }
        public RepositoryUsers(VGListContext context) {  Context = context; }

        public User Login(string email) {
            MailUser mailUsuario = new MailUser(email);
            if (mailUsuario == null) {
                throw new Exception("Please, enter a valid E-Mail");
            }
            return Context.Users
                            .Where(u => u.Mail.Value == mailUsuario.Value)
                            .SingleOrDefault();
        }

        public void Add(User user) {
            bool mailExists = Context.Set<User>().Any(u => u.Mail.Value == user.Mail.Value);
            if (mailExists) {
                throw new Exception("There already another User with this E-Mail");
            }
            Context.Add(user);
            Context.SaveChanges();
        }
        public User FindById(int id) {
            return Context.Users
                .Where(u => u.Id == id)
                .SingleOrDefault();
        }
        public void Remove(int id) {
            User toErase = FindById(id);
            if (toErase == null) throw new Exception("The provided Id does not correspond with any User");
            Context.Users.Remove(toErase);
            Context.SaveChanges();
        }

        public void Update(User updatedUser) {
            Context.Users.Update(updatedUser);
            Context.SaveChanges();
        }

        public List<User> FindAll(int ammount) {
            List<User> users = Context.Users.Take(ammount).ToList();
            return users;
        }
        public List<User> FindSuggestions(string partialQuery, int ammount) {
            List<User> users = Context.Users.Where(u=> u.UserName.StartsWith(partialQuery)).Take(ammount).ToList();
            return users;
        }
    }
}
