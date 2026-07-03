using LogicaNegocio.Dominio;
using LogicaNegocio.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs {
    public static class UserMapper {
        public static User ToUserFromRegister(DTOUserRegister userDTO) {
            User user = new User(
                userName: userDTO.UserName,
                profileUrl: "",
                personalProfile: new Profile(""),
                role: "NormalUser",
                name: userDTO.Name,
                dateOfBirth: userDTO.DateOfBirth,
                mail: userDTO.Mail,
                password: userDTO.Password
            );
            return user;
        }

        public static DTOUserLogged ToLoggedFromUser(User user) {
            DTOUserLogged userLogged = new DTOUserLogged() {
                Mail = user.Mail.Value,
                Role = user.Role
            };
            return userLogged;
        }
    }
}
