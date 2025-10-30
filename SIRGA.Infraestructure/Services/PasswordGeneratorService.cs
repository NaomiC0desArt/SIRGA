using SIRGA.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Infraestructure.Services
{
    public class PasswordGeneratorService : IPasswordGeneratorService
    {
        public string GenerateTemporaryPassword()
        {
            // Genera una contraseña temporal segura de 12 caracteres Incluye mayúsculas, minúsculas, números y un carácter especial

            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#$%";

            var password = new StringBuilder();

            // Asegurar al menos un carácter de cada tipo
            password.Append(GetRandomChar(uppercase));
            password.Append(GetRandomChar(lowercase));
            password.Append(GetRandomChar(digits));
            password.Append(GetRandomChar(special));

            // Rellenar el resto con caracteres aleatorios
            string allChars = lowercase + uppercase + digits + special;
            for (int i = 0; i < 8; i++)
            {
                password.Append(GetRandomChar(allChars));
            }

            // Mezclar los caracteres
            return new string(password.ToString().OrderBy(x => RandomNumberGenerator.GetInt32(100)).ToArray());
        }

        private char GetRandomChar(string chars)
        {
            int index = RandomNumberGenerator.GetInt32(chars.Length);
            return chars[index];
        }
    }
}
