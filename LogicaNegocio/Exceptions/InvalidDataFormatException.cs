using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Exceptions {
    public class InvalidDataFormatException : Exception {
        public InvalidDataFormatException() { }

        public InvalidDataFormatException(string message) : base(message) { }

        public InvalidDataFormatException(string mensaje, Exception innerException) : base(mensaje, innerException) {
        }
    }
}
