

using SIRGA.Application.Interfaces.Services;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Infraestructure.Services
{
    public class MatriculaGeneratorService : IMatriculaGeneratorService
    {
        private readonly IEstudianteRepository _estudianteRepository;

        public MatriculaGeneratorService(IEstudianteRepository estudianteRepository)
        {
            _estudianteRepository = estudianteRepository;
        }

        public async Task<string> GenerateNextMatriculaAsync()
        {
            var lastMatricula = await _estudianteRepository.GetLastMatriculaAsync();

            if (string.IsNullOrEmpty(lastMatricula))
            {
                return "0001";
            }

            if (int.TryParse(lastMatricula, out int lastNumber))
            {
                int nextNumber = lastNumber + 1;
                return nextNumber.ToString("D4"); 
            }

            throw new InvalidOperationException("No se pudo generar la siguiente matrícula.");
        }
    }
}
