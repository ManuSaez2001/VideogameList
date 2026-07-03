using LogicaNegocio.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.VOs {
    public class MailUser {
        public string Value { get; init; }

        public MailUser(string value) {
            this.Value = value;
            Validar();
        }

        private MailUser() {
        }

        private void Validar() {
            string validados = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.1234567890";

            if (this.Value != null && this.Value.Contains("@") && this.Value.EndsWith(".com")) {
                string substringMail = Value.Substring(0, Value.IndexOf("@"));
                bool substringValid = true;
                for (int i = 0; i < substringMail.Length; i++) {
                    if (!validados.Contains(substringMail[i])) {
                        substringValid = false; break;
                    }
                }
                if (substringMail.Length < 3 || !substringValid) {
                    throw new InvalidDataFormatException("Correo Invalido");
                }
            } else {
                throw new InvalidDataFormatException("Correo Invalido");
            }

        }
        public override bool Equals(object? obj) {
            MailUser otro = obj as MailUser;
            if (otro == null) return false;
            return otro.Value == Value;
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }
}
