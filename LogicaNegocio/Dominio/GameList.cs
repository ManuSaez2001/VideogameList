using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio {
    public class GameList {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Entry> Entries {  get; set; } = new List<Entry>();
        public User Owner { get; set; }
        public GameList() {
        }
        public GameList(string name, string description, User owner) {
            CreatedDate = DateTime.Now;
            this.Name = name;
            this.Description = description;
            Owner = owner;
        }
        public GameList(string name, User owner) {
            CreatedDate = DateTime.Now;
            this.Name = name;
            Owner = owner;
            Description = "";
        }
    }
}
