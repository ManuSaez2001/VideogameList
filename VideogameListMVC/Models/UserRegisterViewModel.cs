using System.ComponentModel.DataAnnotations;

namespace VideogameListMVC.Models {
    public class UserRegisterViewModel {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Email Address")]
        public string Mail { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
