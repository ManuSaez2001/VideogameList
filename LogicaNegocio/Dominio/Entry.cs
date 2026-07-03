using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio {
    public class Entry {
        public int Id { get; set; }
        public string GameId { get; set; }
        public int Rating { get; set; }
        public int Rank {  get; set; }
        public int Tier { get; set; }
        public DateTime DateOfEntry {  get; set; }
        public DateTime DateOfStarted { get; set; }
        public DateTime DateOfFinished { get; set; }
        public User Owner { get; set; }

        public Entry() {
            DateOfEntry = DateTime.Now;
            DateOfStarted = DateTime.Now;
            DateOfFinished = DateTime.Now;
        }
        public Entry(string gameId, int rating, int rank, int tier, User owner) {
            GameId = gameId;
            Rating = rating;
            Rank = rank;
            Tier = tier;
            Owner = owner;
            DateOfEntry = DateTime.Now;
            DateOfStarted = DateTime.Now;
            DateOfFinished = DateTime.Now;
        }

        public Entry(string gameId, User owner) { 
            GameId = gameId;
            Owner = owner;
            DateOfEntry = DateTime.Now;
            DateOfStarted = DateTime.Now;
            DateOfFinished = DateTime.Now;
        }

    }
}
