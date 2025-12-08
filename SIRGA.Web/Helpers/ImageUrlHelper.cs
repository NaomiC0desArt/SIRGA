namespace SIRGA.Web.Helpers
{
    public class ImageUrlHelper
    {
        private readonly string _apiBaseUrl;

        public ImageUrlHelper(IConfiguration configuration)
        {
            // Lee la URL del API desde appsettings.json
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7166";
        }

        public string GetFullImageUrl(string relativePath)
        {
            // Si no hay ruta, retornar null
            if (string.IsNullOrEmpty(relativePath))
                return null;

            // Si ya es una URL completa, retornarla tal cual
            if (relativePath.StartsWith("http://") || relativePath.StartsWith("https://"))
                return relativePath;

            // Asegurar que empiece con /
            if (!relativePath.StartsWith("/"))
                relativePath = "/" + relativePath;

            // Retornar la URL completa: https://localhost:7166/uploads/actividades/imagen.jpg
            return $"{_apiBaseUrl}{relativePath}";
        }
    }
}
