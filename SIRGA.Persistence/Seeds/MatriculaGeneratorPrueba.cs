namespace SIRGA.Persistence.Seeds
{
    public static class MatriculaGeneratorPrueba
    {
        private static int _currentYear = DateTime.Now.Year;
        private static int _lastConsecutive = 0;
        private static bool _initialized = false;
        private static readonly object _lock = new();
        private const int CONSECUTIVE_LENGTH = 4;

        public static void Initialize(string lastMatricula)
        {
            if (_initialized) return;

            lock (_lock)
            {
                int maxConsecutive = 0;

                // Si la matrícula existe y tiene el formato NUEVO (YYYYCCCCC)
                if (!string.IsNullOrEmpty(lastMatricula) && lastMatricula.Length >= (4 + CONSECUTIVE_LENGTH))
                {
                    if (int.TryParse(lastMatricula.Substring(0, 4), out int year))
                    {
                        // 1. Si la última matrícula encontrada es del AÑO ACTUAL, continuamos su secuencia.
                        if (year == _currentYear && int.TryParse(lastMatricula.Substring(4), out int consecutive))
                        {
                            maxConsecutive = consecutive;
                        }
                        // Si la última matrícula es de un año anterior (ej. 2024...), empezamos en 1 para 2025.
                    }
                }
                // Si la matrícula existe y tiene el formato ANTIGUO (solo consecutivo, ej. "0004")
                else if (!string.IsNullOrEmpty(lastMatricula) && lastMatricula.Length <= CONSECUTIVE_LENGTH)
                {
                    if (int.TryParse(lastMatricula, out int consecutive))
                    {
                        // 2. Si es antigua, tomamos su valor como el máximo.
                        //    Esto asegura que los nuevos números (YYYYCCCCC) empiecen *después* de 
                        //    los antiguos. Por ejemplo, si el último era "0004", el nuevo será "202500005".
                        maxConsecutive = consecutive;
                    }
                }

                _lastConsecutive = maxConsecutive;
                _initialized = true;
            }
        }

        public static string Generar()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("MatriculaGeneratorPrueba debe ser inicializado antes de generar matrículas.");
            }

            lock (_lock)
            {
                _lastConsecutive++;
                // Usamos _currentYear que está fijado al año en que se ejecuta el seeder.
                return _currentYear.ToString() + _lastConsecutive.ToString().PadLeft(CONSECUTIVE_LENGTH, '0');
            }
        }
    }
}
