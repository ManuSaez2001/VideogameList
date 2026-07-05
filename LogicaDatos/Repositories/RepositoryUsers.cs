using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositories;
using LogicaNegocio.VOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Repositories {
    public class RepositoryUsers : IRepositoryUsers {
        public VGListContext Context { get; set; }
        public RepositoryUsers(VGListContext context) {  Context = context; }

        public async Task<User> GetByMailAsync(string email) {
            MailUser mailUsuario = new MailUser(email);
            if (mailUsuario == null) {
                throw new Exception("Please, enter a valid E-Mail");
            }
            return await Context.Users
                            .Where(u => u.Mail.Value == mailUsuario.Value)
                            .SingleOrDefaultAsync();
        }

        public async Task AddAsync(User user) {
            bool mailExists = await Context.Set<User>().AnyAsync(u => u.Mail.Value == user.Mail.Value);
            if (mailExists) {
                throw new Exception("There already another User with this E-Mail");
            }
            Context.Add(user);
            await Context.SaveChangesAsync();
        }
        public async Task<User> FindByIdAsync(int id) {
            return await Context.Users
                .Where(u => u.Id == id)
                .SingleOrDefaultAsync();
        }
        public async Task RemoveAsync(int id) {
            User toErase = await FindByIdAsync(id);
            if (toErase == null) throw new Exception("The provided Id does not correspond with any User");
            Context.Users.Remove(toErase);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User updatedUser) {
            Context.Users.Update(updatedUser);
            await Context.SaveChangesAsync();
        }

        public async Task<List<User>> FindAllAsync(int ammount) {
            List<User> users = await Context.Users.Take(ammount).ToListAsync();
            return users;
        }
        public async Task<List<User>> FindSuggestionsAsync(string partialQuery, int ammount) {
            List<User> users = await Context.Users.Where(u=> u.UserName.StartsWith(partialQuery)).Take(ammount).ToListAsync();
            return users;
        }
    }
}
