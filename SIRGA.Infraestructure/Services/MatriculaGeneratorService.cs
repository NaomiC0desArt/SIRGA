

using SIRGA.Application.Interfaces.Services;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Infraestructure.Services
{
    public class MatriculaGeneratorService : IMatriculaGeneratorService
    {
        private readonly IEstudianteRepository _estudianteRepository;
        private const int CONSECUTIVE_LENGTH = 4;

        public MatriculaGeneratorService(IEstudianteRepository estudianteRepository)
        {
            _estudianteRepository = estudianteRepository;
        }

        public async Task<string> GenerateNextMatriculaAsync()
        {
            string currentYear = DateTime.Now.Year.ToString();
            var lastMatricula = await _estudianteRepository.GetLastMatriculaAsync();
            int maxConsecutive = 0;

            if (!string.IsNullOrEmpty(lastMatricula) && lastMatricula.Length >= (4 + CONSECUTIVE_LENGTH))
            {
                if (int.TryParse(lastMatricula.Substring(0, 4), out int year))
                {
                    // Si la última matrícula encontrada es del AÑO ACTUAL, continuamos su secuencia.
                    if (year == DateTime.Now.Year && int.TryParse(lastMatricula.Substring(4), out int consecutive))
                    {
                        maxConsecutive = consecutive;
                    }
                    // Si es de un año anterior, maxConsecutive permanece en 0 (la nueva secuencia empieza en 1).
                }
            }

            else if (!string.IsNullOrEmpty(lastMatricula) && lastMatricula.Length <= CONSECUTIVE_LENGTH)
            {
                if (int.TryParse(lastMatricula, out int consecutive))
                {
                    // Si es antigua, tomamos su valor como el máximo para continuar la secuencia.
                    maxConsecutive = consecutive;
                }
            }

            int nextConsecutive = maxConsecutive + 1;

            // Formato: AÑO + Consecutivo rellenado a 4 dígitos
            return currentYear + nextConsecutive.ToString($"D{CONSECUTIVE_LENGTH}");
        }
    }
}
