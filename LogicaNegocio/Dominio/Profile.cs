using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio {
    public class Profile {
        public int Id { get; set; }
        public List<Comment> Comments { get; set; }
        public string Description { get; set; }
        public List<GameList> GameLists { get; set; }

        public Profile() { }
        public Profile(string description) {
            Description = description;
            GameLists = new List<GameList>();
            Comments = new List<Comment>();
        }
    }
}
