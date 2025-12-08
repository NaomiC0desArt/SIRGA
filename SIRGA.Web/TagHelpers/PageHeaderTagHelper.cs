using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SIRGA.Web.TagHelpers
{
    /// Tag Helper para crear el encabezado principal de página con icono, título y fecha
    /// Uso: <page-header title="Panel de Administración" username="@Model.UserName" 
    ///                   icon="shield" color="blue" show-date="true" />

    [HtmlTargetElement("page-header")]
    public class PageHeaderTagHelper : TagHelper
    {
        public string Title { get; set; } = "";
        public string Username { get; set; } = "";
        public string Icon { get; set; } = "shield";
        public string Color { get; set; } = "blue";
        public bool ShowDate { get; set; } = false;

        private readonly Dictionary<string, string> ColorGradients = new()
        {
            { "blue", "from-blue-500 to-blue-600|bg-blue-50|border-blue-200|text-blue-700" },
            { "green", "from-green-500 to-green-600|bg-green-50|border-green-200|text-green-700" },
            { "purple", "from-purple-500 to-purple-600|bg-purple-50|border-purple-200|text-purple-700" },
            { "orange", "from-orange-500 to-orange-600|bg-orange-50|border-orange-200|text-orange-700" }
        };

        private readonly Dictionary<string, string> IconPaths = new()
        {
            { "shield", "M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" },
            { "book", "M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" },
            { "user", "M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" }
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "bg-white rounded-2xl shadow-xl p-8 border border-gray-100 mb-8");

            var colorParts = GetColorParts();
            var iconPath = IconPaths.ContainsKey(Icon) ? IconPaths[Icon] : IconPaths["shield"];

            var dateSection = ShowDate ? $@"
                <div class='{colorParts[1]} px-4 py-2 rounded-lg border {colorParts[2]}'>
                    <p class='text-sm {colorParts[3]} font-medium'>
                        {DateTime.Now.ToString("dddd, dd MMMM yyyy")}
                    </p>
                </div>
            " : "";

            var layout = ShowDate ? "justify-between" : "text-center";

            output.Content.SetHtmlContent($@"
                <div class='flex items-center {layout} flex-wrap gap-4'>
                    <div class='flex items-center'>
                        <div class='h-16 w-16 bg-gradient-to-r {colorParts[0]} rounded-full flex items-center justify-center mr-4 shadow-lg'>
                            <svg class='w-8 h-8 text-white' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
                                <path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='{iconPath}'></path>
                            </svg>
                        </div>
                        <div>
                            <h1 class='text-3xl font-bold text-gray-900'>{Title}</h1>
                            <p class='text-gray-600 mt-1'>Bienvenido, <strong>{Username}</strong></p>
                        </div>
                    </div>
                    {dateSection}
                </div>
            ");
        }

        private string[] GetColorParts()
        {
            return ColorGradients.ContainsKey(Color)
                ? ColorGradients[Color].Split('|')
                : ColorGradients["blue"].Split('|');
        }
    }
}
