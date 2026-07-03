using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs {
    public class DTOUserRegister {
        public string UserName { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Mail {  get; set; }
        public string Password { get; set; }
    }
}
