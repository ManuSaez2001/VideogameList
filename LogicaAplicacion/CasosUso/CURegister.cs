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
    public class CURegister : ICURegister {
        IRepositoryUsers RepoUser {  get; set; }
        public CURegister(IRepositoryUsers repoUser) {
            RepoUser = repoUser;
        }

        public void Register(DTOUserRegister userDTO) {
            User user = UserMapper.ToUserFromRegister(userDTO);
            //user.IsValid();
            user.Password = Security.HashearPasswordPBKDF2(userDTO.Password);
            RepoUser.Add(user);
        }
    }
}
