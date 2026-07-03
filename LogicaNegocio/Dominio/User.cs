using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio {
    public class User : Person {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProfileUrl { get; set; }
        public Profile PersonalProfile { get; set; }
        public string Role { get; set; }

        public User() { }
        public User(string userName, string profileUrl, Profile personalProfile, string role, 
                    string name, DateTime dateOfBirth, string mail, string password) 
            : base(name, dateOfBirth, mail, password) {
            UserName = userName;
            ProfileUrl = profileUrl;
            PersonalProfile = personalProfile;
            Role = role;
        }

        public void IsValid() {
            throw new NotImplementedException();
        }
    }
}
