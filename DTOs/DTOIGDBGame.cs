using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs {
    public class DTOIGDBGame {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public int? Cover { get; set; }
        public double? Rating { get; set; }
        public long? FirstReleaseDate { get; set; }
        public string? Summary { get; set; }
    }
}
