using LogicaNegocio.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio {
    public class Person {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public MailUser Mail { get; set; }
        public string Password { get; set; }

        public Person() { }
        public Person(string name, DateTime dateOfBirth, string mail, string password) {
            Name = name;
            DateOfBirth = dateOfBirth;
            Mail = new MailUser(mail);
            Password = password;
        }
    }
}
