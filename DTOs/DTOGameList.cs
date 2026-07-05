using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs {
    public class DTOGameList {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required string OwnerEmail { get; set; }
    }
}
