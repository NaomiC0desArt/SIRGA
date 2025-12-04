using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Seeds
{
    public static class EmailGenerator
    {
        public static string GenerateEstudianteEmail(string matricula)
        {
            // Formato: {matricula}{anoDeIngreso}@SIGA.edu.do
            // Ejemplo: 000120231@SIGA.edu.do
            return $"{matricula}@SIGA.edu.do";
        }

        public static string GenerateProfesorEmail(string firstName, string lastName)
        {
            // Formato: {nombre}{apellido}@SIGA.edu.do
            // Ejemplo: juanperez@SIGA.edu.do

            var cleanFirstName = RemoveAccentsAndSpaces(firstName);
            var cleanLastName = RemoveAccentsAndSpaces(lastName);

            return $"{cleanFirstName}{cleanLastName}@SIGA.edu.do";
        }

        private static string RemoveAccentsAndSpaces(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Remover acentos
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Remover espacios y caracteres especiales
            result = Regex.Replace(result, @"[^a-zA-Z]", "");

            return result;
        }
    }
}

