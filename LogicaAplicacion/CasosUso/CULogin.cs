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
    public class CULogin : ICULogin {
        IRepositoryUsers Repo {  get; set; }
        public CULogin(IRepositoryUsers repo) { Repo = repo; }
        public async Task<DTOUserLogged> LoginAsync(DTOUserLogin userDTO) {
            User user = await Repo.GetByMailAsync(userDTO.Mail);
            if (!Security.VerificarPasswordPBKDF2(userDTO.Password, user.Password)) {
                throw new Exception("Invalid password");
            }
            return UserMapper.ToLoggedFromUser(user);
        }
    }
}
