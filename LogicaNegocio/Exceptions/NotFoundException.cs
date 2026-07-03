using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Exceptions {
    public class NotFoundException : Exception {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string mensaje, Exception innerException) : base(mensaje, innerException) {
        }
    }
}
