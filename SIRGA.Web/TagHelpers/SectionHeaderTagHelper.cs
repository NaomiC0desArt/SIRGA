using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SIRGA.Web.TagHelpers
{
    /// Tag Helper para crear encabezados de sección consistentes
    /// Uso: <section-header title="Gestión Académica" icon="menu" color="blue" />
    [HtmlTargetElement("section-header")]
    public class SectionHeaderTagHelper : TagHelper
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "menu";
        public string Color { get; set; } = "blue";
        public string Subtitle { get; set; } = "";

        private readonly Dictionary<string, string> ColorClasses = new()
        {
            { "blue", "text-blue-500" },
            { "green", "text-green-500" },
            { "purple", "text-purple-500" },
            { "orange", "text-orange-500" },
            { "indigo", "text-indigo-500" },
            { "teal", "text-teal-500" },
            { "pink", "text-pink-500" }
        };

        private readonly Dictionary<string, string> IconPaths = new()
        {
            { "menu", "M4 6h16M4 10h16M4 14h16M4 18h16" },
            { "grid", "M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" },
            { "users", "M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" },
            { "shield", "M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" }
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "h2";
            var colorClass = ColorClasses.ContainsKey(Color) ? ColorClasses[Color] : ColorClasses["blue"];
            var iconPath = IconPaths.ContainsKey(Icon) ? IconPaths[Icon] : IconPaths["menu"];

            output.Attributes.SetAttribute("class", "text-2xl font-bold text-gray-900 mb-6 flex items-center");

            var subtitleHtml = !string.IsNullOrEmpty(Subtitle)
                ? $"<p class='text-sm text-gray-500 ml-9'>{Subtitle}</p>"
                : "";

            output.Content.SetHtmlContent($@"
                <svg class='w-6 h-6 mr-3 {colorClass}' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
                    <path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='{iconPath}'></path>
                </svg>
                {Title}
                {subtitleHtml}
            ");
        }
    }
}
